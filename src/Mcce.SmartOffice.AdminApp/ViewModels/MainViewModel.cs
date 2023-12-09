using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;

        public override string Title => "The Smart Office (Admin)";

        public MainViewModel(
            IAuthService authService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task Workspaces()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceListPage));
        }

        [RelayCommand]
        public async Task WorkspaceData()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceDataPage));
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
