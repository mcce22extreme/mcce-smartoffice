using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Models;
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
        public async Task<BookingModel[]> GetBookings()
        {
            return await _bookingManager.GetBookings();
        }

        [HttpGet("{bookingId}")]
        public async Task<BookingModel> GetBooking(int bookingId)
        {
            return await _bookingManager.GetBooking(bookingId);
        }

        [HttpPost]
        public async Task<BookingModel> CreateBooking([FromBody] SaveBookingModel model)
        {
            return await _bookingManager.CreateBooking(model, Url.ActionLink(nameof(ActivateBooking)));
        }

        [HttpDelete("{bookingId}")]
        public async Task DeleteBooking(int bookingId)
        {
            await _bookingManager.DeleteBooking(bookingId);
        }

        [HttpGet("activate")]
        public async Task<IActionResult> ActivateBooking([FromQuery] string activationCode)
        {
            await _bookingManager.ActivateBooking(activationCode);

            return Ok();
        }

        private string CreateUrl(int userImageId)
        {
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/userimage/{userImageId}/content";
            return url;
        }
    }
}
