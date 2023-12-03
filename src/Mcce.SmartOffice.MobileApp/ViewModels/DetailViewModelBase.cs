using System.ComponentModel;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public interface IDetailViewModelBase
    {
        bool HasUnsavedData { get; }
    }

    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModelBase
    {
        protected bool IsLoaded { get; set; } = false;

        public bool HasUnsavedData { get; protected set; } = false;

        protected DetailViewModelBase(INavigationService navigationService)
            : base(navigationService)
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
    }
}
