using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(
            IAuthService authService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService, authService)
        {
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
                await NavigationService.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
