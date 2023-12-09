using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Bookings.Enums;
using Mcce.SmartOffice.Bookings.Messages;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Common.Constants;
using Mcce.SmartOffice.Common.Services;
using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Enums;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Mcce.SmartOffice.Bookings.Managers
{
    public interface IBookingManager
    {
        Task<BookingModel[]> GetBookings(bool onlyMyBookings = false, DateTime? startDateTime = null, DateTime? endDateTime = null);

        Task<BookingModel> GetBooking(string bookingNumber);

        Task<BookingModel> CreateBooking(SaveBookingModel model);

        Task<BookingModel> ConfirmBooking(string bookingNumber);

        Task<BookingModel> RejectBooking(string bookingNumber);

        Task DeleteBooking(string bookingNumber);

        Task ActivateBooking(string bookingNumber);
    }

    public class BookingManager : IBookingManager
    {
        private const int ACTIVATION_CODE_LENGTH = 40;

        private static readonly Random _random = new Random();

        private readonly string _frontendUrl;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthContextAccessor _contextAccessor;
        private readonly IMessageService _messageService;

        public BookingManager(
            string frontendUrl,
            AppDbContext dbContext,
            IMapper mapper,
            IAuthContextAccessor contextAccessor,
            IMessageService messageService)
        {
            _frontendUrl = frontendUrl;
            _dbContext = dbContext;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _messageService = messageService;
        }

        public async Task<BookingModel[]> GetBookings(bool onlyMyBookings = false, DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var bookingsQuery = _dbContext.Bookings
                .OrderBy(x => x.StartDateTime)
                .AsQueryable();

            if (startDateTime.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(x => x.StartDateTime >= startDateTime.Value);
            }

            if (endDateTime.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(x => x.EndDateTime <= endDateTime.Value);
            }

            if (onlyMyBookings == true)
            {
                bookingsQuery = bookingsQuery.Where(x => x.UserName == currentUser.UserName);
            }

            var bookings = await bookingsQuery
                .ProjectTo<BookingModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            // Suppress user information for other users if user is not admin
            if (!currentUser.IsAdmin)
            {
                foreach (var booking in bookings.Where(x => x.UserName != currentUser.UserName))
                {
                    booking.UserName = booking.FirstName = booking.LastName = booking.Creator = booking.Modifier = null;
                }
            }

            return bookings.ToArray();
        }

        public async Task<BookingModel> GetBooking(string bookingNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                throw new NotFoundException($"Could not find booking '{bookingNumber}'.");
            }

            if (!currentUser.IsAdmin && booking.UserName != currentUser.UserName)
            {
                throw new ForbiddenException("You are not allowed to perform this action!");
            }

            return _mapper.Map<BookingModel>(booking);
        }

        public async Task<BookingModel> CreateBooking(SaveBookingModel model)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            // Validate booking
            if (model.StartDateTime > model.EndDateTime)
            {
                throw new ValidationException("End date of booking must not be before start date of booking!");
            }

            var collision = await HasCollision(model.WorkspaceNumber, model.StartDateTime, model.EndDateTime);

            if (collision)
            {
                throw new ValidationException("A collision occurred during booking the workspace! The workspace has already been booked by another user in the specified time.");
            }

            var booking = _mapper.Map<Booking>(model);

            booking.BookingNumber = GenerateBookingNumber();
            booking.FirstName = currentUser.FirstName;
            booking.LastName = currentUser.LastName;
            booking.UserName = currentUser.UserName;
            booking.State = BookingState.Pending;

            await _dbContext.Bookings.AddAsync(booking);

            await _dbContext.SaveChangesAsync();

            Log.Debug($"Created booking '{booking.BookingNumber}' for workspace '{booking.WorkspaceNumber}' and user '{booking.UserName}' with booking number '{booking.BookingNumber}'.");

            await _messageService.Publish(
                MessageTopics.TOPIC_BOOKING_CREATED,
                new
                {
                    currentUser.UserName,
                    booking.BookingNumber,
                    booking.WorkspaceNumber,
                });

            return _mapper.Map<BookingModel>(booking);
        }

        public async Task DeleteBooking(string bookingNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking != null)
            {
                if (!currentUser.IsAdmin && booking.UserName != currentUser.UserName)
                {
                    throw new ForbiddenException("You are not allowed to perform this action!");
                }

                _dbContext.Bookings.Remove(booking);

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<bool> HasCollision(string workspaceNumber, DateTime startDateTime, DateTime endDateTime)
        {
            return await _dbContext.Bookings
                .AnyAsync(x =>
                    x.WorkspaceNumber == workspaceNumber &&
                    startDateTime < x.EndDateTime &&
                    endDateTime > x.StartDateTime);
        }

        private string GenerateBookingNumber()
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

        public async Task ActivateBooking(string bookingNumber)
        {
            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                throw new NotFoundException($"Could not find booking '{bookingNumber}'.");
            }

            if (booking.State != BookingState.Confirmed && booking.State != BookingState.Activated)
            {
                throw new ValidationException($"The booking '{booking.BookingNumber}' has not yet been confirmed and therefore cannot be activated!");
            }

            booking.State = BookingState.Activated;

            await _dbContext.SaveChangesAsync();

            await _messageService.Publish(
                MessageTopics.TOPIC_BOOKING_ACTIVATED,
                new BookingActivatedMessage(booking.BookingNumber, booking.WorkspaceNumber, booking.UserName));
        }

        public async Task<BookingModel> ConfirmBooking(string bookingNumber)
        {
            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                throw new NotFoundException($"Could not find booking '{bookingNumber}'.");
            }

            booking.State = BookingState.Confirmed;

            await _dbContext.SaveChangesAsync(auditInfoUpdateMode: AuditInfoUpdateMode.Suppress);

            await _messageService.Publish(
                string.Format(MessageTopics.TOPIC_BOOKING_CONFIRMED, booking.UserName),
                new
                {
                    booking.UserName,
                    booking.BookingNumber,
                    booking.WorkspaceNumber,
                    ActivationUrl = $"{_frontendUrl}/booking/{booking.BookingNumber}/activate"
                });

            return await GetBooking(bookingNumber);
        }

        public async Task<BookingModel> RejectBooking(string bookingNumber)
        {
            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                throw new NotFoundException($"Could not find booking '{bookingNumber}'.");
            }

            booking.State = BookingState.Rejected;

            await _dbContext.SaveChangesAsync(auditInfoUpdateMode: AuditInfoUpdateMode.Suppress);

            await _messageService.Publish(
                string.Format(MessageTopics.TOPIC_BOOKING_REJECTED, booking.UserName),
                new
                {
                    booking.UserName,
                    booking.BookingNumber,
                    booking.WorkspaceNumber,
                });

            return await GetBooking(bookingNumber);
        }
    }
}
