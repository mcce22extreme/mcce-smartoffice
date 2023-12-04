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

        [ObservableProperty]
        private bool _isBusy;

        public ViewModelBase(INavigationService navigationService, IDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }

        public abstract Task Activate();

        public virtual Task<bool> CanGoBack()
        {
            return Task.FromResult(true);
        }
    }
}
