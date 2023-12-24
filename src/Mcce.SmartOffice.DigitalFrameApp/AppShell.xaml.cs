using MauiIcons.Core;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.Common.Services;
using Mcce.SmartOffice.DigitalFrameApp.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Models;
using Mcce.SmartOffice.DigitalFrameApp.Pages;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.DigitalFrameApp
{
    public partial class AppShell : Shell, IMessageHandler
    {
        private readonly IAppConfig _appConfig;
        private readonly IMessageService _messageService;
        private readonly ISessionManager _sessionManager;
        private readonly INavigationService _navigationService;

        public string[] SupportedTopics => new[] { "mcce-smartoffice/workspace/workspace-simulator/activate/userimages" };

        public AppShell(IAppConfig appConfig, IMessageService messageService, ISessionManager sessionManager, INavigationService navigationService)
        {
            InitializeComponent();

            _ = new MauiIcon();

            Routing.RegisterRoute(nameof(PrepareSessionPage), typeof(PrepareSessionPage));
            Routing.RegisterRoute(nameof(SlideshowPage), typeof(SlideshowPage));
            Routing.RegisterRoute(nameof(EndSessionPage), typeof(EndSessionPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

            _appConfig = appConfig;
            _messageService = messageService;
            _sessionManager = sessionManager;
            _navigationService = navigationService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _messageService.Subscribe($"mcce-smartoffice/workspace/{_appConfig.WorkspaceNumber}/activate/userimages", this);

            DeviceDisplay.Current.KeepScreenOn = true;
        }

        public async Task Handle(string topic, string payload)
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                var model = JsonConvert.DeserializeObject<WorkspaceActivatedModel>(payload);

                _sessionManager.StartSession(model);

                await _navigationService.GoToAsync(nameof(PrepareSessionPage));
            });
        }
    }
}
