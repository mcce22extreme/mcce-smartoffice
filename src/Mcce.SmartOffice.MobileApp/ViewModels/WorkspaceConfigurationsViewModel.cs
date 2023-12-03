using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class WorkspaceConfigurationsViewModel : ObservableObject
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;

        public string Title
        {
            get
            {
                var title = "My Configurations";

                if (WorkspaceConfigurations?.Count > 0)
                {
                    title += $" ({WorkspaceConfigurations.Count})";
                }

                return title;
            }
        }


        [ObservableProperty]
        private List<WorkspaceConfigurationModel> _workspaceConfigurations;

        [ObservableProperty]
        private WorkspaceConfigurationModel _selectedWorkspaceConfiguration;

        [ObservableProperty]
        private bool _isBusy;

        public WorkspaceConfigurationsViewModel(IWorkspaceConfigurationManager workspaceConfigurationManager)
        {
            _workspaceConfigurationManager = workspaceConfigurationManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        [RelayCommand(CanExecute = nameof(CanLoadWorkspaceConfigurations))]
        public async Task LoadWorkspaceConfigurations()
        {
            if (CanLoadWorkspaceConfigurations())
            {
                IsBusy = true;

                try
                {
                    SelectedWorkspaceConfiguration = null;
                    var configurations = await _workspaceConfigurationManager.GetWorkspaceConfigurations();

                    WorkspaceConfigurations = new List<WorkspaceConfigurationModel>(configurations);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool CanLoadWorkspaceConfigurations()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanDeleteWorkspaceConfiguration))]
        public async Task DeleteWorkspaceConfiguration()
        {
            if (CanDeleteWorkspaceConfiguration())
            {
                IsBusy = true;
                try
                {
                    var result = await Application.Current.MainPage.DisplayAlert("Delete Configuration?", "Do you really want to delete the selected workspace configuration?", "Yes", "No");

                    if (result)
                    {
                        await _workspaceConfigurationManager.DeleteBooking(SelectedWorkspaceConfiguration.WorkspaceNumber);

                        WorkspaceConfigurations.Remove(SelectedWorkspaceConfiguration);
                        OnPropertyChanged(nameof(Title));
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool CanDeleteWorkspaceConfiguration()
        {
            return !IsBusy && SelectedWorkspaceConfiguration != null;
        }
    }
}
