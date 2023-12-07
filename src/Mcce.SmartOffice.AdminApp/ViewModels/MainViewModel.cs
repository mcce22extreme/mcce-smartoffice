using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;

        public override string Title => "The Smart Office (Admin)";

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
        public async Task Workspaces()
        {
            await NavigationService.GoToAsync(nameof(WorkspaceListPage));
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
