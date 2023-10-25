using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.MobileApp.Constants;
using Mcce22.SmartOffice.MobileApp.Pages;
using Mcce22.SmartOffice.MobileApp.Services;

namespace Mcce22.SmartOffice.MobileApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private bool _isBusy;

        public MainViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                if (await _authService.PerformLogin())
                {
                    await Shell.Current.GoToAsync(Routes.Account);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
