using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAccountManager _accountManager;

        public LoginViewModel(
            IAuthService authService,
            IAccountManager accountManager,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService, authService)
        {
            _accountManager = accountManager;
        }

        public override Task Activate()
        {
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (await AuthService.SignIn())
            {
                var accountInfo = await _accountManager.GetAccountInfo();

                if(accountInfo?.IsAdmin == true)
                {
                    await NavigationService.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    await AuthService.SignOut();
                    await DialogService.ShowErrorMessage("The user used to sign in either does not have access to Smart Office or is no administrator. Please sign in with an administrator user.");
                }
                
            }
        }
    }
}
