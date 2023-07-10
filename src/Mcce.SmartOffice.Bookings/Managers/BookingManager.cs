using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Bookings.Services;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetBookings();

        Task<BookingModel> GetBooking(int bookingId);

        Task<BookingModel> CreateBooking(SaveBookingModel model, string activationLink);

        Task DeleteBooking(int bookingId);
    }

    public class BookingManager : IBookingManager
    {
        private const int ACTIVATION_CODE_LENGTH = 40;

        private static readonly Random _random = new Random();

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingManager(
            AppDbContext dbContext,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _emailService = emailService;
            _contextAccessor = contextAccessor;
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
            var currentUser = _contextAccessor.GetUserInfo();

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

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Bookings.AddAsync(booking);

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            // send confirmation email
            await _emailService.SendMail(
                new BookingConfirmationModel
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    StartDateTime = booking.StartDateTime,
                    EndDateTime = booking.EndDateTime,
                    ActivationCode = booking.ActivationCode,
                    WorkspaceNumber = model.WorkspaceNumber,
                }, activationLink);

            return await GetBooking(booking.Id);
        }

        public async Task DeleteBooking(int bookingId)
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Bookings
                .Where(x => x.Id == bookingId)
                .ExecuteDeleteAsync();

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();
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

        //public async Task ActivateBooking(string activationCode, Func<int, string> imageUrlFunc)
        //{
        //    await Semaphore.WaitAsync();

        //    try
        //    {
        //        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.ActivationCode == activationCode);

        //        if (booking == null)
        //        {
        //            throw new ValidationException($"Could not find booking for activationcode '{activationCode}'!");
        //        }

        //        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == booking.UserId);
        //        var configuration = await _dbContext.WorkspaceConfigurations.FirstOrDefaultAsync(x => x.WorkspaceId == booking.WorkspaceId && x.UserId == booking.UserId);
        //        var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == booking.WorkspaceId);
        //        var userImages = await _dbContext.UserImages
        //        .Where(x => x.UserId == booking.UserId)
        //        .ToListAsync();

        //        var model = new WorkspaceActivationModel
        //        {
        //            BookingId = booking.Id,
        //            WorkspaceId = booking.WorkspaceId,
        //            WorkspaceNumber = workspace.WorkspaceNumber,
        //            UserId = booking.UserId,
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            DeskHeight = configuration.DeskHeight,
        //            UserImageUrls = userImages.Select(x => imageUrlFunc(x.Id)).ToArray(),
        //        };

        //        await _messageService.Publish(Topics.DEVICE_ACTIVATED, model);

        //        booking.Activated = true;

        //        using var tx = await _dbContext.Database.BeginTransactionAsync();

        //        await _dbContext.SaveChangesAsync();

        //        await tx.CommitAsync();
        //    }
        //    finally
        //    {
        //        Semaphore.Release();
        //    }
        //}

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
    }
}
