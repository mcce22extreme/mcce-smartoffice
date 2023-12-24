using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class BookingDetailViewModel : DetailViewModelBase
    {
        private readonly IBookingManager _bookingManager;
        private readonly IWorkspaceManager _workspaceManager;

        public override string Title => "New Booking";

        [ObservableProperty]
        private List<WorkspaceModel> _workspaces;

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;

        [ObservableProperty]
        private DateTime? _selectedStartDate;

        [ObservableProperty]
        private TimeSpan? _selectedStartTime;

        [ObservableProperty]
        private DateTime? _selectedEndDate;

        [ObservableProperty]
        private TimeSpan? _selectedEndTime;

        public BookingDetailViewModel(
            IBookingManager bookingManager,
            IWorkspaceManager workspaceManager,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthService authService)
            : base(navigationService, dialogService, authService)
        {
            _bookingManager = bookingManager;
            _workspaceManager = workspaceManager;

            SelectedStartDate = DateTime.Now;
            SelectedStartTime = DateTime.Now.TimeOfDay;
            SelectedEndDate = DateTime.UtcNow;
            SelectedEndTime = DateTime.Now.AddMinutes(15).TimeOfDay;
        }

        public override async Task Activate()
        {
            await LoadWorkspaces();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            CreateBookingCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadWorkspaces()
        {
            IsBusy = true;

            try
            {
                await HandleException(async () =>
                {
                    SelectedWorkspace = null;

                    var workspaces = await _workspaceManager.GetWorkspaces();

                    Workspaces = new List<WorkspaceModel>(workspaces);

                    IsLoaded = true;
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanCreateBooking))]
        private async Task CreateBooking()
        {
            if (CanCreateBooking())
            {
                try
                {
                    await HandleException(async () =>
                    {
                        IsBusy = true;

                        var startDateTime =  SelectedStartDate.Value.Date.Add(SelectedStartTime.Value);
                        var endDateTime = SelectedEndDate.Value.Date.Add(SelectedEndTime.Value);

                        await _bookingManager.CreateBooking(
                            SelectedWorkspace.WorkspaceNumber,
                            startDateTime,
                            endDateTime);

                        HasUnsavedData = false;

                        await NavigationService.GoToAsync($"///{nameof(MainPage)}/{nameof(BookingListPage)}");
                    });
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanCreateBooking()
        {
            return !IsBusy &&
                SelectedWorkspace != null &&
                SelectedStartDate.HasValue &&
                SelectedStartTime.HasValue &&
                SelectedEndDate.HasValue &&
                SelectedEndTime.HasValue;
        }
    }
}
