using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        public override string Title => "The Smart Office";

        [ObservableProperty]
        private string _fullName;

        public MainViewModel(
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
                IsBusy = true;

                var accountInfo = await _accountManager.GetAccountInfo();

                FullName = $"{accountInfo.FirstName} {accountInfo.LastName}";
            }
            catch (HttpRequestException)
            {
                await _authService.SignIn();
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task CreateBooking()
        {
            await NavigationService.GoToAsync($"{nameof(BookingDetailPage)}");
        }

        [RelayCommand]
        private async Task Bookings()
        {
            await NavigationService.GoToAsync($"{nameof(BookingListPage)}");
        }

        [RelayCommand]
        public async Task UserImages()
        {
            await NavigationService.GoToAsync($"{nameof(UserImageListPage)}");
        }

        [RelayCommand]
        public async Task WorkspaceConfigurations()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceConfigurationListPage));
        }

        [RelayCommand]
        public async Task SignOut()
        {
            var result = await DialogService.ShowConfirmationDialog("Sign Out?", "Do you really want to sign out?");

            if (result)
            {
                await _authService.SignOut();

                await NavigationService.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
