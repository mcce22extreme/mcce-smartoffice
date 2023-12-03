using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;
using Mcce.SmartOffice.MobileApp.Pages;
using Mcce.SmartOffice.MobileApp.Services;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class WorkspaceConfigurationDetailViewModel : DetailViewModelBase
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;
        private readonly IWorkspaceManager _workspaceManager;

        public override string Title => "Workspace Configuration";

        private string _workspaceNumber;

        public string WorkspaceNumber
        {
            get { return _workspaceNumber; }
            set
            {
                _workspaceNumber = value;
                IsNew = string.IsNullOrEmpty(WorkspaceNumber);
            }
        }

        [ObservableProperty]
        private ObservableCollection<WorkspaceModel> _workspaces = new ObservableCollection<WorkspaceModel>();

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;

        [ObservableProperty]
        private int _deskHeight;

        [ObservableProperty]
        private bool _isNew;

        public WorkspaceConfigurationDetailViewModel(
            IWorkspaceConfigurationManager workspaceConfigurationManager,
            IWorkspaceManager workspaceManager,
            INavigationService navigationService)
            : base(navigationService)
        {
            _workspaceConfigurationManager = workspaceConfigurationManager;
            _workspaceManager = workspaceManager;
        }

        public override async Task Activate()
        {
            IsBusy = true;

            try
            {
                SelectedWorkspace = null;

                var workspaceConfigurations = await _workspaceConfigurationManager.GetWorkspaceConfigurations();

                if (string.IsNullOrEmpty(WorkspaceNumber))
                {
                    var workspaces = await _workspaceManager.GetWorkspaces();

                    // Only add workspaces for that no configuration exists, yet
                    Workspaces = new ObservableCollection<WorkspaceModel>(workspaces.Where(x => !workspaceConfigurations.Any(c => c.WorkspaceNumber == x.WorkspaceNumber)));
                }
                else
                {
                    var configuration = workspaceConfigurations.FirstOrDefault(x => x.WorkspaceNumber == WorkspaceNumber);
                    Workspaces.Add(new WorkspaceModel { WorkspaceNumber = WorkspaceNumber });
                    SelectedWorkspace = Workspaces.FirstOrDefault();
                    DeskHeight = configuration.DeskHeight;
                }

                IsLoaded = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            SaveWorkspaceConfigurationCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanSaveWorkspaceConfiguration))]
        private async Task SaveWorkspaceConfiguration()
        {
            if (CanSaveWorkspaceConfiguration())
            {
                try
                {
                    IsBusy = true;


                    await _workspaceConfigurationManager.SaveWorkspaceConfigurations(SelectedWorkspace.WorkspaceNumber, DeskHeight);

                    HasUnsavedData = false;

                    await NavigationService.GoBackAsync();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Oops, something went wrong!", ex.Message, "Close");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanSaveWorkspaceConfiguration()
        {
            return !IsBusy &&
                SelectedWorkspace != null &&
                DeskHeight >= 70 &&
                DeskHeight <= 120;
        }
    }
}
