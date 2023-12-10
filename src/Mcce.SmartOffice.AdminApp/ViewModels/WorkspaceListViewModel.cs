using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.AdminApp.Pages;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class WorkspaceListViewModel : ViewModelBase
    {
        private readonly IWorkspaceManager _workspaceManager;

        public override string Title
        {
            get
            {
                var title = "Workspaces";

                if (Workspaces?.Count > 0)
                {
                    title += $"  ({Workspaces?.Count})";
                }

                return title;
            }
        }

        [ObservableProperty]
        private ObservableCollection<WorkspaceModel> _workspaces;

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;

        public WorkspaceListViewModel(
            IWorkspaceManager workspaceManager,
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthService authService)
            : base(navigationService, dialogService, authService)
        {
            _workspaceManager = workspaceManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            LoadWorkspacesCommand.NotifyCanExecuteChanged();
            CreateWorkspaceCommand.NotifyCanExecuteChanged();
            EditWorkspaceCommand.NotifyCanExecuteChanged();
            DeleteWorkspaceCommand.NotifyCanExecuteChanged();
        }

        public override async Task Activate()
        {
            await LoadWorkspaces();
        }

        [RelayCommand(CanExecute = nameof(CanLoadWorksapces))]
        private async Task LoadWorkspaces()
        {
            if (CanLoadWorksapces())
            {
                try
                {
                    await HandleException(async () =>
                    {
                        IsBusy = true;

                        SelectedWorkspace = null;

                        var workspaces = await _workspaceManager.GetWorkspaces();

                        Workspaces = new ObservableCollection<WorkspaceModel>(workspaces);

                        OnPropertyChanged(nameof(Title));
                    });
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanLoadWorksapces()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCreateWorkspace))]
        private async Task CreateWorkspace()
        {
            if (CanCreateWorkspace())
            {
                try
                {
                    IsBusy = true;

                    await NavigationService.GoToAsync(nameof(WorkspaceDetailPage));
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanCreateWorkspace()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanEditWorkspace))]
        private async Task EditWorkspace()
        {
            if (CanEditWorkspace())
            {
                try
                {
                    IsBusy = true;

                    await NavigationService.GoToAsync($"{nameof(WorkspaceDetailPage)}?WorkspaceNumber={SelectedWorkspace.WorkspaceNumber}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanEditWorkspace()
        {
            return !IsBusy && SelectedWorkspace != null;
        }

        [RelayCommand(CanExecute = nameof(CanDeleteWorkspace))]
        private async Task DeleteWorkspace()
        {
            if (CanDeleteWorkspace())
            {
                if (await DialogService.ShowConfirmationDialog("Delete workspace?", "Do you realy want to delete the selected workspace?"))
                {
                    try
                    {
                        await HandleException(async () =>
                        {
                            IsBusy = true;

                            await _workspaceManager.DeleteWorkspaces(SelectedWorkspace.WorkspaceNumber);

                            Workspaces.Remove(SelectedWorkspace);
                            SelectedWorkspace = null;
                            OnPropertyChanged(nameof(Title));
                        });
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        private bool CanDeleteWorkspace()
        {
            return !IsBusy && SelectedWorkspace != null;
        }
    }
}
