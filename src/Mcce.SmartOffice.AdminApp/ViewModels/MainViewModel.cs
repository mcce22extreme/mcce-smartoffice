using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public override string Title => "The Smart Office (Admin)";

        public MainViewModel(
            IAuthService authService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService, authService)
        {
        }

        [RelayCommand]
        private async Task Workspaces()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceListPage));
        }

        [RelayCommand]
        private async Task WorkspaceData()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceDataPage));
        }

        [RelayCommand]
        private async Task Bookings()
        {
            await NavigationService.GoToAsync(nameof(BookingListPage));
        }

        [RelayCommand]
        private async Task SignOut()
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
