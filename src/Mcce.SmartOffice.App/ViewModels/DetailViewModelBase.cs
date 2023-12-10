using System.ComponentModel;
using Mcce.SmartOffice.App.Services;

namespace Mcce.SmartOffice.App.ViewModels
{
    public interface IDetailViewModel : IViewModel
    {
        bool HasUnsavedData { get; }

        Task<bool> CanGoBack();
    }

    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        protected bool IsLoaded { get; set; } = false;

        public bool HasUnsavedData { get; protected set; } = false;

        protected DetailViewModelBase(INavigationService navigationService, IDialogService dialogService, IAuthService authService)
            : base(navigationService, dialogService, authService)
        {
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (IsLoaded && e.PropertyName != nameof(IsBusy))
            {
                HasUnsavedData = true;
            }
        }

        public override async Task<bool> CanGoBack()
        {
            if (HasUnsavedData)
            {
                return await DialogService.ShowConfirmationDialog("Cancel?", "Do you really want to cancel?");
            }
            else
            {
                return await base.CanGoBack();
            }
        }
    }
}
