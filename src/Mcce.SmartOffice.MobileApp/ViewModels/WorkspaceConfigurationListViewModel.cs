using System.Collections.ObjectModel;
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
    public partial class WorkspaceConfigurationListViewModel : ViewModelBase
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;

        public override string Title
        {
            get
            {
                var title = "My Configs";

                if (WorkspaceConfigurations?.Count > 0)
                {
                    title += $" ({WorkspaceConfigurations.Count})";
                }

                return title;
            }
        }

        [ObservableProperty]
        private ObservableCollection<WorkspaceConfigurationModel> _workspaceConfigurations;

        [ObservableProperty]
        private WorkspaceConfigurationModel _selectedWorkspaceConfiguration;

        public WorkspaceConfigurationListViewModel(
            IWorkspaceConfigurationManager workspaceConfigurationManager,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthService authService)
            : base(navigationService, dialogService, authService)
        {
            _workspaceConfigurationManager = workspaceConfigurationManager;
        }

        public override async Task Activate()
        {
            await LoadWorkspaceConfigurations();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            CreateWorkspaceConfigurationCommand.NotifyCanExecuteChanged();
            EditWorkspaceConfigurationCommand.NotifyCanExecuteChanged();
            LoadWorkspaceConfigurationsCommand.NotifyCanExecuteChanged();
            DeleteWorkspaceConfigurationCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanLoadWorkspaceConfigurations))]
        private async Task LoadWorkspaceConfigurations()
        {
            if (CanLoadWorkspaceConfigurations())
            {
                try
                {
                    await HandleException(async () =>
                    {
                        IsBusy = true;

                        SelectedWorkspaceConfiguration = null;

                        var configurations = await _workspaceConfigurationManager.GetWorkspaceConfigurations();

                        WorkspaceConfigurations = new ObservableCollection<WorkspaceConfigurationModel>(configurations);

                        OnPropertyChanged(nameof(Title));
                    });                   
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanLoadWorkspaceConfigurations()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCreateWorkspaceConfiguration))]
        private async Task CreateWorkspaceConfiguration()
        {
            if (CanCreateWorkspaceConfiguration())
            {
                await NavigationService.GoToAsync(nameof(WorkspaceConfigurationDetailPage));
            }
        }

        private bool CanCreateWorkspaceConfiguration()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanEditWorkspaceConfiguration))]
        private async Task EditWorkspaceConfiguration()
        {
            if (CanCreateWorkspaceConfiguration())
            {
                await NavigationService.GoToAsync($"{nameof(WorkspaceConfigurationDetailPage)}?WorkspaceNumber={SelectedWorkspaceConfiguration.WorkspaceNumber}");
            }
        }

        private bool CanEditWorkspaceConfiguration()
        {
            return !IsBusy && SelectedWorkspaceConfiguration != null;
        }

        [RelayCommand(CanExecute = nameof(CanDeleteWorkspaceConfiguration))]
        private async Task DeleteWorkspaceConfiguration()
        {
            if (CanDeleteWorkspaceConfiguration())
            {                
                try
                {
                    var result = await DialogService.ShowConfirmationDialog("Delete Configuration?", "Do you really want to delete the selected workspace configuration?");

                    if (result)
                    {
                        await HandleException(async () =>
                        {
                            IsBusy = true;

                            await _workspaceConfigurationManager.DeleteBooking(SelectedWorkspaceConfiguration.WorkspaceNumber);

                            WorkspaceConfigurations.Remove(SelectedWorkspaceConfiguration);

                            OnPropertyChanged(nameof(Title));
                        });
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanDeleteWorkspaceConfiguration()
        {
            return !IsBusy && SelectedWorkspaceConfiguration != null;
        }
    }
}
