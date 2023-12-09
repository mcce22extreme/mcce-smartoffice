using Mcce.SmartOffice.Bookings.Managers;
using Mcce.SmartOffice.Common.Constants;
using Mcce.SmartOffice.Common.Services;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Bookings.Handlers
{
    public class BookingValidatedHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_BOOKING_VALIDATED };

        public BookingValidatedHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(string topic, string payload)
        {
            var validationResult = JsonConvert.DeserializeObject<BookingValidationResult>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var bookingManager = scope.ServiceProvider.GetRequiredService<IBookingManager>();

            if (validationResult.Confirmed)
            {
                await bookingManager.ConfirmBooking(validationResult.BookingNumber);
            }
            else
            {
                await bookingManager.RejectBooking(validationResult.BookingNumber);
            }
        }

        private class BookingValidationResult
        {
            public string BookingNumber { get; set; }

            public string UserName { get; set; }

            public string WorkspaceNumber { get; set; }

            public bool Confirmed { get; set; }
        }
    }
}
