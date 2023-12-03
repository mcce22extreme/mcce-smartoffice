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
    public partial class WorkspaceConfigurationListViewModel : ViewModelBase
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;
        private readonly INavigationService _navigationService;

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

        [ObservableProperty]
        private bool _isBusy;

        public WorkspaceConfigurationListViewModel(IWorkspaceConfigurationManager workspaceConfigurationManager, INavigationService navigationService)
            : base(navigationService)
        {
            _workspaceConfigurationManager = workspaceConfigurationManager;
            _navigationService = navigationService;
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
                IsBusy = true;

                try
                {
                    SelectedWorkspaceConfiguration = null;
                    var configurations = await _workspaceConfigurationManager.GetWorkspaceConfigurations();

                    WorkspaceConfigurations = new ObservableCollection<WorkspaceConfigurationModel>(configurations);

                    OnPropertyChanged(nameof(Title));
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

        private bool CanDeleteWorkspaceConfiguration()
        {
            return !IsBusy && SelectedWorkspaceConfiguration != null;
        }
    }
}
