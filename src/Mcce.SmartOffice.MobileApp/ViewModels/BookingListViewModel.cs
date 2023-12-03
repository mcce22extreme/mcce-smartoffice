using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class BookingListViewModel : ViewModelBase
    {
        private readonly IBookingManager _bookingManager;

        public override string Title => "My Bookings";

        [ObservableProperty]
        private List<BookingModel> _bookings;

        [ObservableProperty]
        private BookingModel _selectedBooking;

        public BookingListViewModel(IBookingManager bookingManager, INavigationService navigationService)
            : base(navigationService)
        {
            _bookingManager = bookingManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            CreateBookingCommand.NotifyCanExecuteChanged();
            LoadBookingsCommand.NotifyCanExecuteChanged();
            CancelBookingCommand.NotifyCanExecuteChanged();
            ActivateBookingCommand.NotifyCanExecuteChanged();
        }

        public override async Task Activate()
        {
            await LoadBookings();
        }

        [RelayCommand(CanExecute = nameof(CanCreateBooking))]
        private async Task CreateBooking()
        {
            if (CanCreateBooking())
            {
                await NavigationService.GoToAsync($"{nameof(BookingDetailPage)}");
            }
        }

        private bool CanCreateBooking()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanLoadBookings))]
        private async Task LoadBookings()
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

        private bool CanLoadBookings()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanActivateBooking))]
        private async Task ActivateBooking()
        {
            if (CanActivateBooking())
            {
                var result = await Application.Current.MainPage.DisplayAlert("Activate Booking?", "Do you really want to activate this booking?", "Yes", "No");

                if (result)
                {
                    IsBusy = true;
                    try
                    {
                        await _bookingManager.ActivateBooking(SelectedBooking.BookingNumber);

                        await LoadBookings();
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }                
            }
        }

        private bool CanActivateBooking()
        {
            return !IsBusy && (SelectedBooking?.State == Enums.BookingState.Confirmed || SelectedBooking?.State == Enums.BookingState.Activated);
        }

        [RelayCommand(CanExecute = nameof(CanCancelBooking))]
        private async Task CancelBooking()
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

        private bool CanCancelBooking()
        {
            return !IsBusy && SelectedBooking != null;
        }
    }
}
