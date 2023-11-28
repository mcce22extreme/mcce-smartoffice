using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        [ObservableProperty]
        private string _fullName;

        public MainViewModel(IAuthService authService, IAccountManager accountManager)
        {
            _authService = authService;
            _accountManager = accountManager;
        }

        public async Task LoadAccountInfo()
        {
            try
            {
                var accountInfo = await _accountManager.GetAccountInfo();

                FullName = $"{accountInfo.FirstName} {accountInfo.LastName}";
            }
            catch (HttpRequestException)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        [RelayCommand]
        public async Task SignOut()
        {
            await _authService.SignOut();

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
