using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class CreateBookingViewModel : ViewModelBase
    {
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IBookingManager _bookingManager;

        private List<BookingModel> _allBookings = new List<BookingModel>();

        [ObservableProperty]
        private DateTime _startDateTime;

        [ObservableProperty]
        private DateTime _endDateTime;

        [ObservableProperty]
        private List<WorkspaceModel> _workspaces;

        [ObservableProperty]
        private List<BookingModel> _bookings;

        [ObservableProperty]
        private bool _workspacesAvailable;

        private WorkspaceModel _selectedWorkspace;
        public WorkspaceModel SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set
            {
                if (SetProperty(ref _selectedWorkspace, value))
                {
                    UpdateBookings();
                }
            }
        }

        public event EventHandler WorkspaceAvailabilityUpdated;

        public CreateBookingViewModel(IWorkspaceManager workspaceManager, IBookingManager bookingManager)
        {
            _workspaceManager = workspaceManager;
            _bookingManager = bookingManager;

            var dateTimeNow = DateTime.Now;
            StartDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 6, 0, 0);
            EndDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 18, 0, 0);
        }

        protected override void UpdateCommandStates()
        {
            base.UpdateCommandStates();

            UpdateAvailabilityCommand.NotifyCanExecuteChanged();
            CreateBookingCommand.NotifyCanExecuteChanged();
        }

        public async Task Load()
        {
            try
            {
                IsBusy = true;

                var workspaces = await _workspaceManager.GetList();

                Workspaces = new List<WorkspaceModel>(workspaces);

                UpdateAvailability();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanUpdateAvailability()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanUpdateAvailability))]
        private async void UpdateAvailability()
        {
            try
            {
                IsBusy = true;

                SelectedWorkspace = null;

                _allBookings = new List<BookingModel>(await _bookingManager.GetList());

                foreach (var workspace in Workspaces)
                {
                    workspace.IsAvailable = !_allBookings.Any(x =>
                        x.WorkspaceNumber == workspace.WorkspaceNumber &&
                        x.StartDateTime.Date == StartDateTime.Date &&
                        ((x.StartDateTime.TimeOfDay == StartDateTime.TimeOfDay && x.EndDateTime.TimeOfDay == EndDateTime.TimeOfDay) ||
                        (x.StartDateTime.TimeOfDay <= StartDateTime.TimeOfDay && x.EndDateTime.TimeOfDay > StartDateTime.TimeOfDay) ||
                        (x.StartDateTime.TimeOfDay >= StartDateTime.TimeOfDay && x.EndDateTime.TimeOfDay <= EndDateTime.TimeOfDay)));
                }

                WorkspaceAvailabilityUpdated?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanCreateBooking()
        {
            return !IsBusy && SelectedWorkspace?.IsAvailable == true;
        }

        [RelayCommand(CanExecute = nameof(CanCreateBooking))]
        private async Task CreateBooking()
        {
            try
            {
                IsBusy = true;

                await _bookingManager.Create(new BookingModel
                {
                    StartDateTime = StartDateTime,
                    EndDateTime = EndDateTime,
                    WorkspaceNumber = SelectedWorkspace.WorkspaceNumber
                });
            }
            finally
            {
                IsBusy = false;
            }

            UpdateAvailability();
        }

        private void UpdateBookings()
        {
            try
            {
                IsBusy = true;

                if (SelectedWorkspace == null)
                {
                    Bookings = new List<BookingModel>();
                }
                else
                {
                    Bookings = _allBookings.Where(x => x.WorkspaceNumber == SelectedWorkspace.WorkspaceNumber).ToList();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
