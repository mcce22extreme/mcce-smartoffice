using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        public LoadingViewModel(
            IAuthService authService,
            IAccountManager accountManager,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _authService = authService;
            _accountManager = accountManager;
        }

        public override async Task Activate()
        {
            try
            {
                var accountInfo = await _accountManager.GetAccountInfo();

                if (accountInfo?.IsAdmin == true)
                {
                    await NavigationService.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    await _authService.SignOut();
                    await NavigationService.GoToAsync($"//{nameof(LoginPage)}");
                }
                
            }
            catch (HttpRequestException)
            {
                await NavigationService.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
