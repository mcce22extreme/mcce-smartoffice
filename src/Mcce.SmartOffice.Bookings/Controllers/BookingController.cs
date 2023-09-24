﻿using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Bookings.Models;
using Mcce.SmartOffice.Core.Extensions;
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
        public async Task<BookingModel[]> GetBookings()
        {
            return await _bookingManager.GetBookings();
        }

        [HttpPost]
        public async Task<BookingModel> CreateBooking([FromBody] SaveBookingModel model)
        {
            return await _bookingManager.CreateBooking(model, CreateUrl);
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

        private string CreateUrl(string bookingNumber)
        {
            var origin = Request.Headers.Origin.FirstOrDefault();

            if (origin.HasValue())
            {
                return $"{origin}/booking/{bookingNumber}/activate";
            }
            else
            {
                return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/booking/{bookingNumber}/activate";
            }
        }
    }
}
