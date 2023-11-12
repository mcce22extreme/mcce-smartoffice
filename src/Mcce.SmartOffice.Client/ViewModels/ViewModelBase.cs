using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            UpdateCommandStates();
        }

        protected virtual void UpdateCommandStates()
        {
        }
    }
}
