using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class BookingListViewModel : ListViewModelBase<BookingModel>
    {
        private readonly IBookingManager _bookingManager;

        [ObservableProperty]
        private bool _isAdmin;

        public BookingListViewModel(
            IBookingManager bookingManager,
            IDialogService dialogService)
            : base(dialogService)
        {
            _bookingManager = bookingManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ActivateBookingCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanActivateBooking))]
        protected async Task ActivateBooking()
        {
            if (CanActivateBooking())
            {
                await _bookingManager.ActivateBooking(SelectedItem.BookingNumber);
                await Reload();
            }
        }

        protected bool CanActivateBooking()
        {
            return !IsBusy && SelectedItem?.State == Enums.BookingState.Confirmed;
        }

        protected override async Task OnDelete()
        {
            await _bookingManager.Delete(SelectedItem.BookingNumber);
        }

        protected override async Task<BookingModel[]> OnReload()
        {
            return await _bookingManager.GetList();
        }
    }
}
