using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Services;
using Mcce.SmartOffice.WorkspaceConfigurations.Managers;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.WorkspaceConfigurations
{
    public class BookingActivatedHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageService _messageService;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_BOOKING_ACTIVATED };

        public BookingActivatedHandler(IServiceScopeFactory serviceScopeFactory, IMessageService messageService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageService = messageService;
        }

        public async Task Handle(string topic, string payload)
        {
            var bookingInfo = JsonConvert.DeserializeObject<BookingInfo>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var workspaceConfigurationManager = scope.ServiceProvider.GetService<IWorkspaceConfigurationManager>();

            var config = await workspaceConfigurationManager.GetWorkspaceConfigurationByUserName(bookingInfo.WorkspaceNumber, bookingInfo.UserName);

            if(config != null)
            {
                await _messageService.Publish(string.Format(MessageTopics.TOPIC_WORKSPACE_ACTIVATE_WORKSPACECONFIGURATION, bookingInfo.WorkspaceNumber), new
                {
                    bookingInfo.UserName,
                    bookingInfo.WorkspaceNumber,
                    config.DeskHeight
                });
            }
        }

        private class BookingInfo
        {
            public string UserName { get; set; }

            public string WorkspaceNumber { get; set; }
        }
    }
}
