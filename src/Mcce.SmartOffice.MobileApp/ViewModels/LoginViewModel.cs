using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;

        public LoginViewModel(
            IAuthService authService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _authService = authService;
        }

        public override Task Activate()
        {
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (await _authService.SignIn())
            {
                await NavigationService.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
