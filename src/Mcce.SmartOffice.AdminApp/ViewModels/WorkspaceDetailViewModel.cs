using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class WorkspaceDetailViewModel : DetailViewModelBase
    {
        private readonly IWorkspaceManager _workspaceManager;
        private readonly Regex _regex = new Regex("^[a-zA-Z0-9-]+$", RegexOptions.Compiled);

        public override string Title => "Create Workspace";

        [ObservableProperty]
        private string _workspaceNumber;

        public WorkspaceDetailViewModel(
            IWorkspaceManager workspaceManager,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _workspaceManager = workspaceManager;
        }

        public override Task Activate()
        {
            IsLoaded = true;

            return Task.CompletedTask;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            SaveWorkspaceCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanSaveWorkspace))]
        private async Task SaveWorkspace()
        {
            if (CanSaveWorkspace())
            {
                try
                {
                    if (!_regex.IsMatch(WorkspaceNumber))
                    {
                        await DialogService.ShowDialog("Validation error", "The workspace number must only contain alphanumeric characters or the special character '-'!");
                        return;
                    }

                    IsBusy = true;

                    var model = new WorkspaceModel
                    {
                        WorkspaceNumber = WorkspaceNumber
                    };

                    await _workspaceManager.CreateWorkspaces(model);

                    HasUnsavedData = false;

                    await NavigationService.GoBackAsync();
                }
                catch (Exception ex)
                {
                    await DialogService.ShowErrorMessage(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanSaveWorkspace()
        {
            return !IsBusy && !string.IsNullOrEmpty(WorkspaceNumber);
        }
    }
}
