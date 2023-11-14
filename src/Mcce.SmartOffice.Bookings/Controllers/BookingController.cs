using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Bookings.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : Controller
    {
        private readonly IBookingManager _bookingManager;

        public BookingController(IBookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        [HttpGet]
        public async Task<BookingModel[]> GetBookings(DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            return await _bookingManager.GetBookings(startDateTime, endDateTime);
        }

        [HttpGet("details")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task<BookingModel[]> GetBookingDetails(bool includeAll = false, DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            return await _bookingManager.GetBookingDetails(includeAll, startDateTime, endDateTime);
        }

        [HttpPost]
        public async Task<BookingModel> CreateBooking([FromBody] SaveBookingModel model)
        {
            return await _bookingManager.CreateBooking(model);
        }

        [HttpDelete("{bookingNumber}")]
        public async Task DeleteBooking(string bookingNumber)
        {
            await _bookingManager.DeleteBooking(bookingNumber);
        }

        [AllowAnonymous]
        [HttpGet("{bookingNumber}/activate")]
        public async Task<IActionResult> ActivateBooking(string bookingNumber)
        {
            await _bookingManager.ActivateBooking(bookingNumber);

            return Ok();
        }
    }
}
