using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public override string Title => "The Smart Office";

        public MainViewModel(
            IAuthService authService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService, authService)
        {
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
                await AuthService.SignOut();

                await NavigationService.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}
