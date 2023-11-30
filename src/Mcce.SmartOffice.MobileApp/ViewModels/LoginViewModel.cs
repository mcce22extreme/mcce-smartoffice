using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class LoginViewModel
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (await _authService.SignIn())
            {
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
