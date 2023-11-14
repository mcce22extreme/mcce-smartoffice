using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public interface IViewModel
    {
        Task Activate();
    }

    public partial class ViewModelBase : ObservableObject, IViewModel
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

        public virtual Task Activate()
        {
            return Task.CompletedTask;
        }
    }
}
