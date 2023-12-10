using CommunityToolkit.Mvvm.ComponentModel;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.DigitalFrameApp.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Pages;

namespace Mcce.SmartOffice.DigitalFrameApp.ViewModels
{
    public partial class PrepareSessionViewModel : ViewModelBase
    {
        private readonly ISessionManager _sessionManager;

        [ObservableProperty]
        private string _fullName;

        public PrepareSessionViewModel(
            ISessionManager sessionManager,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _sessionManager = sessionManager;
        }

        public override async Task Activate()
        {
            FullName = _sessionManager.GetCurrentUserName();

            await _sessionManager.PrepareSession();

            await NavigationService.GoToAsync(nameof(SlideshowPage));
        }
    }
}
