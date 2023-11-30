using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public class LoadingViewModel
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        public LoadingViewModel(IAuthService authService, IAccountManager accountManager)
        {
            _authService = authService;
            _accountManager = accountManager;
        }

        public async Task VerifySession()
        {
            // try to load account info

            try
            {
                await _accountManager.GetAccountInfo();

                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (HttpRequestException)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
