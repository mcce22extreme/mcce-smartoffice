using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        private readonly IAccountManager _accountManager;

        public LoadingViewModel(
            IAuthService authService,
            IAccountManager accountManager,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService, authService)
        {
            _accountManager = accountManager;
        }

        public override async Task Activate()
        {
            try
            {
                await _accountManager.GetAccountInfo();

                await NavigationService.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (HttpRequestException)
            {
                await NavigationService.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
