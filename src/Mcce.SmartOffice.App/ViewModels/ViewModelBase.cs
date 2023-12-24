using CommunityToolkit.Mvvm.ComponentModel;
using Mcce.SmartOffice.App.Services;

namespace Mcce.SmartOffice.App.ViewModels
{
    public interface IViewModel
    {
        string Title { get; }

        bool IsBusy { get; }

        Task Activate();
    }

    public abstract partial class ViewModelBase : ObservableObject, IViewModel
    {
        public virtual string Title { get; }

        protected INavigationService NavigationService { get; }

        public IDialogService DialogService { get; }

        public IAuthService AuthService { get; }

        [ObservableProperty]
        private bool _isBusy;

        public ViewModelBase(INavigationService navigationService, IDialogService dialogService, IAuthService authService = null)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
            AuthService = authService;
        }

        public virtual Task Activate()
        {
            return Task.CompletedTask;
        }

        public virtual Task Deactivate()
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> CanGoBack()
        {
            return Task.FromResult(true);
        }

        protected async Task HandleException(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (AuthService != null && await AuthService.RefreshAccessToken())
                    {
                        await func();
                    }
                    else
                    {
                        await NavigationService.GoToAsync($"//LoginPage");
                    }
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowErrorMessage(ex.Message);
            }
        }
    }
}
