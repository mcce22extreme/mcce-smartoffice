using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;
using Mcce.SmartOffice.MobileApp.Pages;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class CreateBookingViewModel : ObservableObject
    {
        private readonly IBookingManager _bookingManager;
        private readonly IWorkspaceManager _workspaceManager;

        private bool _loaded;

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

        [ObservableProperty]
        private bool _isBusy;

        public bool HasUnsavedData { get; private set; }

        public CreateBookingViewModel(IBookingManager bookingManager, IWorkspaceManager workspaceManager)
        {
            _bookingManager = bookingManager;
            _workspaceManager = workspaceManager;

            SelectedStartDate = DateTime.Now;
            SelectedStartTime = DateTime.Now.TimeOfDay;
            SelectedEndDate = DateTime.UtcNow;
            SelectedEndTime = DateTime.Now.AddMinutes(15).TimeOfDay;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            CreateBookingCommand.NotifyCanExecuteChanged();

            if (_loaded && e.PropertyName != nameof(IsBusy))
            {
                HasUnsavedData = true;
            }
        }

        public async Task LoadWorkspaces()
        {
            IsBusy = true;

            try
            {
                SelectedWorkspace = null;

                var workspaces = await _workspaceManager.GetWorkspaces();

                Workspaces = new List<WorkspaceModel>(workspaces);

                _loaded = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanCreateBooking))]
        public async Task CreateBooking()
        {
            if (CanCreateBooking())
            {
                try
                {
                    IsBusy = true;

                    var startDateTime =  SelectedStartDate.Value.Date.Add(SelectedStartTime.Value);
                    var endDateTime = SelectedEndDate.Value.Date.Add(SelectedEndTime.Value);

                    await _bookingManager.CreateBooking(
                        SelectedWorkspace.WorkspaceNumber,
                        startDateTime,
                        endDateTime);

                    HasUnsavedData = false;

                    await Shell.Current.GoToAsync($"///{nameof(MainPage)}/{nameof(BookingsPage)}");                    
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Opps an error occurred!", ex.Message, "Close");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool CanCreateBooking()
        {
            return !IsBusy &&
                SelectedWorkspace != null &&
                SelectedStartDate.HasValue &&
                SelectedStartTime.HasValue &&
                SelectedEndDate.HasValue &&
                SelectedEndTime.HasValue;
        }

        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync($"..");
        }
    }
}
