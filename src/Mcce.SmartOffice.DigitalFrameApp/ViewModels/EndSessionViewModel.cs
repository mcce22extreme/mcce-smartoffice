using CommunityToolkit.Mvvm.ComponentModel;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.DigitalFrameApp.Managers;

namespace Mcce.SmartOffice.DigitalFrameApp.ViewModels
{
    public partial class EndSessionViewModel : ViewModelBase
    {
        private readonly ISessionManager _sessionManager;

        [ObservableProperty]
        private string _fullName;

        public EndSessionViewModel(
            ISessionManager sessionManager,
            INavigationService navigationService, IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _sessionManager = sessionManager;
        }

        public override async Task Activate()
        {
            await _sessionManager.EndSession();

            await NavigationService.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
