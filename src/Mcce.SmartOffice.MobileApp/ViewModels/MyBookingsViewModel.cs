using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class MyBookingsViewModel : ObservableObject
    {
        private readonly IBookingManager _bookingManager;

        [ObservableProperty]
        private List<BookingModel> _bookings;

        [ObservableProperty]
        private BookingModel _selectedBooking;

        [ObservableProperty]
        private bool _isBusy;

        public MyBookingsViewModel(IBookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            CreateBookingCommand.NotifyCanExecuteChanged();
            LoadBookingsCommand.NotifyCanExecuteChanged();
            CancelBookingCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanCreateBooking))]
        public async Task CreateBooking()
        {
            if (CanCreateBooking())
            {
                await Shell.Current.GoToAsync($"{nameof(CreateBookingPage)}");
            }
        }

        public bool CanCreateBooking()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanLoadBookings))]
        public async Task LoadBookings()
        {
            if (CanLoadBookings())
            {
                IsBusy = true;

                try
                {
                    SelectedBooking = null;
                    var bookings = await _bookingManager.GetMyBookings();

                    Bookings = new List<BookingModel>(bookings);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool CanLoadBookings()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCancelBooking))]
        public async Task CancelBooking()
        {
            if (CanCancelBooking())
            {
                IsBusy = true;
                try
                {
                    var result = await Application.Current.MainPage.DisplayAlert("Cancel Booking?", "Do you really want to cancel your booking?", "Yes", "No");

                    if (result)
                    {
                        await _bookingManager.CancelBooking(SelectedBooking.BookingNumber);
                    }
                }
                finally
                {
                    IsBusy = false;
                }

                await LoadBookings();
            }
        }

        public bool CanCancelBooking()
        {
            return !IsBusy && SelectedBooking != null && SelectedBooking.State == Enums.BookingState.Confirmed;
        }
    }
}
