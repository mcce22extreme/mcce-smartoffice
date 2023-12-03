using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        public LoadingViewModel(IAuthService authService, IAccountManager accountManager, INavigationService navigationService)
            : base(navigationService)
        {
            _authService = authService;
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
