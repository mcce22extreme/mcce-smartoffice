using CommunityToolkit.Mvvm.ComponentModel;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
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

        [ObservableProperty]
        private bool _isBusy;

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public abstract Task Activate();
    }
}
