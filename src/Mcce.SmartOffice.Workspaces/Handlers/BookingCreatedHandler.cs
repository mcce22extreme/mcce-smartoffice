using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Services;
using Mcce.SmartOffice.Workspaces.Managers;
using Newtonsoft.Json;
using Serilog;

namespace Mcce.SmartOffice.Workspaces.Handlers
{
    public class BookingCreatedHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageService _messageService;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_BOOKING_CREATED };

        public BookingCreatedHandler(IServiceScopeFactory serviceScopeFactory, IMessageService messageService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageService = messageService;
        }

        public async Task Handle(string topic, string payload)
        {
            var booking = JsonConvert.DeserializeObject<BookingInfo>(payload);

            Log.Information($"Validate booking request '{booking.BookingNumber}' for user {booking.UserName} and workplace {booking.WorkspaceNumber}...");

            var scope = _serviceScopeFactory.CreateScope();
            var workspaceManager = scope.ServiceProvider.GetRequiredService<IWorkspaceManager>();

            var result = new BookingValidationResult
            {
                BookingNumber = booking.BookingNumber,
                WorkspaceNumber = booking.WorkspaceNumber,
                UserName = booking.UserName,
                Confirmed = await workspaceManager.WorkspaceExists(booking.WorkspaceNumber)
            };

            await _messageService.Publish(MessageTopics.TOPIC_BOOKING_VALIDATED, result);
        }

        private class BookingInfo
        {
            public string UserName { get; set; }

            public string BookingNumber { get; set; }

            public string WorkspaceNumber { get; set; }
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
