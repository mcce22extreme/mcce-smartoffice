using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Services;
using Mcce.SmartOffice.UserImages.Configs;
using Mcce.SmartOffice.UserImages.Managers;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.UserImages.Handlers
{
    public class BookingActivatedHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageService _messageService;
        private readonly AppConfig _appConfig;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_BOOKING_ACTIVATED };

        public BookingActivatedHandler(IServiceScopeFactory serviceScopeFactory, IMessageService messageService, AppConfig appConfig)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageService = messageService;
            _appConfig = appConfig;
        }

        public async Task Handle(string topic, string payload)
        {
            var bookingInfo = JsonConvert.DeserializeObject<BookingInfo>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var userImageManager = scope.ServiceProvider.GetService<IUserImageManager>();

            var userImages = await userImageManager.GetUserImagesByUserName(bookingInfo.UserName, x =>
            {
                return $"{_appConfig.FrontendUrl}/{x}";
            });

            if (userImages.Length > 0)
            {
                await _messageService.Publish(string.Format(MessageTopics.TOPIC_WORKSPACE_ACTIVATE_USERIMAGES, bookingInfo.WorkspaceNumber), new
                {
                    bookingInfo.UserName,
                    bookingInfo.WorkspaceNumber,
                    UserImages = userImages.Select(x => x.Url).ToArray(),
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
