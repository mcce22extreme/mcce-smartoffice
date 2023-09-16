using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Bookings.Services;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Mcce.SmartOffice.Bookings.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetBookings();

        Task<BookingModel> GetBooking(int bookingId);

        Task<BookingModel> CreateBooking(SaveBookingModel model, string activationLink);

        Task DeleteBooking(int bookingId);

        Task ActivateBooking(string activationCode);
    }

    public class BookingManager : IBookingManager
    {
        private const int ACTIVATION_CODE_LENGTH = 40;

        private static readonly Random _random = new Random();

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMessageService _messageService;

        public BookingManager(
            AppDbContext dbContext,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor contextAccessor,
            IMessageService messageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _emailService = emailService;
            _contextAccessor = contextAccessor;
            _messageService = messageService;
        }

        public async Task<BookingModel[]> GetBookings()
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var bookingsQuery = _dbContext.Bookings
                .OrderBy(x => x.StartDateTime)
                .AsQueryable();

            var bookings = await bookingsQuery
                .ProjectTo<BookingModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return bookings.ToArray();
        }

        public async Task<BookingModel> GetBooking(int bookingId)
        {
            var booking = await _dbContext.Bookings
                .ProjectTo<BookingModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == bookingId) ?? throw new EntityNotFoundException<Booking>(bookingId);

            return booking;
        }

        public async Task<BookingModel> CreateBooking(SaveBookingModel model, string activationLink)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            // Validate booking
            if (model.StartDateTime > model.EndDateTime)
            {
                throw new ValidationException("End date of booking must not be before start date of booking!");
            }

            if (model.StartDateTime.Date != model.EndDateTime.Date)
            {
                throw new ValidationException("Stard date and end date must not span multiple days!");
            }

            var collision = await CheckAvailability(model.WorkspaceNumber, model.StartDateTime, model.EndDateTime);

            if (collision)
            {
                throw new ValidationException("A collision occurred during booking the workspace! The workspace has already been booked by another user in the specified time.");
            }

            var booking = _mapper.Map<Booking>(model);

            booking.UserName = currentUser.UserName;
            booking.ActivationCode = GenerateActivationCode();

            await _dbContext.Bookings.AddAsync(booking);

            await _dbContext.SaveChangesAsync();

            Log.Debug($"Created booking '{booking.Id}' for workspace '{booking.WorkspaceNumber}' and user '{booking.UserName}' with activation code '{booking.ActivationCode}'.");

            // send confirmation email
            //await _emailService.SendMail(
            //    new BookingConfirmationModel
            //    {
            //        FirstName = currentUser.FirstName,
            //        LastName = currentUser.LastName,
            //        UserName = currentUser.UserName,
            //        Email = currentUser.Email,
            //        StartDateTime = booking.StartDateTime,
            //        EndDateTime = booking.EndDateTime,
            //        ActivationCode = booking.ActivationCode,
            //        WorkspaceNumber = model.WorkspaceNumber,
            //    }, activationLink);

            return await GetBooking(booking.Id);
        }

        public async Task DeleteBooking(int bookingId)
        {
            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);

            if(booking != null)
            {
                _dbContext.Bookings.Remove(booking);

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<bool> CheckAvailability(string workspaceNumber, DateTime startDateTime, DateTime endDateTime)
        {
            var bookings = await _dbContext.Bookings
                .Where(x => x.WorkspaceNumber == workspaceNumber &&
                            ((x.StartDateTime >= startDateTime || x.EndDateTime <= endDateTime) ||
                             (x.StartDateTime <= startDateTime && x.EndDateTime >= endDateTime)))
                .ToListAsync();

            return bookings.Any();
        }

        private string GenerateActivationCode()
        {
            var bitCount = 6 * ACTIVATION_CODE_LENGTH;
            var byteCount = (int)Math.Ceiling(bitCount / 8f);
            byte[] buffer = new byte[byteCount];
            _random.NextBytes(buffer);

            string guid = Convert.ToBase64String(buffer);

            // Replace URL unfriendly characters
            guid = guid.Replace('+', '-').Replace('/', '_');

            // Trim characters to fit the count
            return guid.Substring(0, ACTIVATION_CODE_LENGTH);
        }

        public async Task ActivateBooking(string activationCode)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var booking = await _dbContext.Bookings
                .FirstOrDefaultAsync(x => x.ActivationCode == activationCode);

            if (booking == null)
            {
                throw new NotFoundException($"Could not find booking for activation code '{activationCode}'.");
            }

            booking.Activated = true;

            await _dbContext.SaveChangesAsync();

            await _messageService.Publish(MessageTopics.TOPIC_BOOKING_ACTIVATED, new
            {
                booking.UserName,
                booking.WorkspaceNumber,
            }); ;
        }
    }
}
