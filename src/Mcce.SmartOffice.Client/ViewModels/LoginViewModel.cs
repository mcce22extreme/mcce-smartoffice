using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IAccountManager _accountManager;
        private readonly IDialogService _dialogService;
        [ObservableProperty]
        private bool _loginVisible = true;

        [ObservableProperty]
        private string _userName = "demo";

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _isAdmin;

        public ICommand LoginCommand { get; }

        public event EventHandler LoginChanged;

        public LoginViewModel(IAuthService authService, IAccountManager accountManager, IDialogService dialogService)
        {
            _authService = authService;
            _accountManager = accountManager;
            _dialogService = dialogService;

            LoginCommand = new RelayCommand(Login);
        }

        public async void Login()
        {
            try
            {
                if (await _authService.Login())
                {
                    var accountInfo = await _accountManager.GetAccountInfo();

                    IsAdmin = accountInfo.IsAdmin;
                    LoginChanged?.Invoke(this, EventArgs.Empty);
                    LoginVisible = false;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(new ErrorViewModel(ex, _dialogService));
            }
        }

        public async void Logout()
        {
            await _authService.Logout();

            LoginVisible = true;
        }
    }
}
