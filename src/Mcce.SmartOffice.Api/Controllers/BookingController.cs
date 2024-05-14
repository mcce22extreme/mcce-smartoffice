using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Api.Controllers
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
        public async Task<BookingModel[]> GetBookings(bool onlyMyBookings = false, DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            return await _bookingManager.GetBookings(onlyMyBookings, startDateTime, endDateTime);
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

        [HttpGet("{bookingNumber}/activate")]
        public async Task<IActionResult> ActivateBooking(string bookingNumber)
        {
            await _bookingManager.ActivateBooking(bookingNumber);

            return Ok();
        }
    }
}
