using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.DigitalFrameApp.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Models;
using Mcce.SmartOffice.DigitalFrameApp.Pages;

namespace Mcce.SmartOffice.DigitalFrameApp.ViewModels
{
    public partial class SlideshowViewModel : ViewModelBase
    {
        public event EventHandler<int> OnUserImageIndexUpdate;

        private readonly ISessionManager _sessionManager;
        private readonly IWorkspaceDataManager _workspaceDataManager;
        private readonly IDispatcherTimer _dispatcherTimer;

        private int _currentImageIndex = 0;

        [ObservableProperty]
        private ObservableCollection<UserImageModel> _userImages = new ObservableCollection<UserImageModel>();

        [ObservableProperty]
        private bool _dataSimulationRunning;

        [ObservableProperty]
        private WorkspaceDataModel _workspaceData;

        public SlideshowViewModel(
            ISessionManager sessionManager,
            IWorkspaceDataManager workspaceDataManager,
            IDispatcherTimer dispatcherTimer,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _sessionManager = sessionManager;
            _workspaceDataManager = workspaceDataManager;
            _dispatcherTimer = dispatcherTimer;

            _dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
            _dispatcherTimer.Tick += OnTimerTicker;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            StartWorkspaceDataSimulationCommand.NotifyCanExecuteChanged();
            StopWorkspaceDataSimulationCommand.NotifyCanExecuteChanged();
        }

        public override Task Activate()
        {
            UserImages.Clear();

            var userImages = _sessionManager.GetUserImages();

            if (userImages.Length > 0)
            {
                foreach (var image in userImages)
                {
                    UserImages.Add(image);
                }
            }

            _dispatcherTimer.Start();

            return base.Activate();
        }

        public override Task Deactivate()
        {
            _dispatcherTimer.Stop();

            UserImages.Clear();

            return base.Deactivate();
        }

        private async void OnTimerTicker(object sender, EventArgs e)
        {
            if (UserImages.Count > 0)
            {
                if (_currentImageIndex >= UserImages.Count)
                {
                    _currentImageIndex = 0;
                }
                else
                {
                    _currentImageIndex += 1;
                }
                
                OnUserImageIndexUpdate?.Invoke(this, _currentImageIndex);
            }

            if (DataSimulationRunning)
            {
                WorkspaceData = await _workspaceDataManager.SendWorkspaceData();
            }
        }

        [RelayCommand]
        private async Task EndSession()
        {
            _dispatcherTimer.Stop();

            UserImages.Clear();

            await NavigationService.GoToAsync(nameof(EndSessionPage));
        }

        [RelayCommand(CanExecute = nameof(CanStartWorkspaceDataSimulation))]
        private void StartWorkspaceDataSimulation()
        {
            DataSimulationRunning = true;
        }

        private bool CanStartWorkspaceDataSimulation()
        {
            return !DataSimulationRunning;
        }

        [RelayCommand(CanExecute = nameof(CanStopWorkspaceDataSimulation))]
        private void StopWorkspaceDataSimulation()
        {
            DataSimulationRunning = false;
        }

        private bool CanStopWorkspaceDataSimulation()
        {
            return DataSimulationRunning;
        }
    }
}
