using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class BookingListViewModel : ViewModelBase
    {
        private readonly IBookingManager _bookingManager;

        public override string Title
        {
            get
            {
                var title = "Bookings";

                if (Bookings?.Count > 0)
                {
                    title += $"  ({Bookings?.Count})";
                }

                return title;
            }
        }

        [ObservableProperty]
        private ObservableCollection<BookingModel> _bookings = new ObservableCollection<BookingModel>();

        [ObservableProperty]
        private BookingModel _selectedBooking;


        public BookingListViewModel(
            IBookingManager bookingManager,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthService authService = null)
            : base(navigationService, dialogService, authService)
        {
            _bookingManager = bookingManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            LoadBookingsCommand.NotifyCanExecuteChanged();
            CancelBookingCommand.NotifyCanExecuteChanged();
        }

        public override async Task Activate()
        {
            await LoadBookings();
        }

        [RelayCommand(CanExecute = nameof(CanLoadBookings))]
        private async Task LoadBookings()
        {
            try
            {
                if (!CanLoadBookings())
                {
                    return;
                }

                await HandleException(async () =>
                {
                    IsBusy = true;

                    Bookings.Clear();
                    SelectedBooking = null;
                    var bookings = await _bookingManager.GetBookings();

                    foreach (var booking in bookings)
                    {
                        Bookings.Add(booking);
                    }

                    OnPropertyChanged(nameof(Title));
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLoadBookings()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCancelBooking))]
        private async Task CancelBooking()
        {
            try
            {
                if (!CanCancelBooking())
                {
                    return;
                }

                if (await DialogService.ShowConfirmationDialog("Cancel booking?", "Do you realy want to cancel the selected booking?"))
                {
                    IsBusy = true;

                    await HandleException(async () =>
                    {
                        await _bookingManager.CancelBooking(SelectedBooking.BookingNumber);

                        Bookings.Remove(SelectedBooking);

                        SelectedBooking = null;

                        OnPropertyChanged(nameof(Title));
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanCancelBooking()
        {
            return !IsBusy && SelectedBooking != null;
        }
    }
}
