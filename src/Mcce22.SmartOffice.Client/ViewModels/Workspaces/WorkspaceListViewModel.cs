using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class WorkspaceListViewModel : ListViewModelBase<WorkspaceModel>
    {
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IAuthService _authService;
        private readonly Timer _updateTimer;

        public WorkspaceListViewModel(IWorkspaceManager workspaceManager, IDialogService dialogService, IAuthService authService)
            : base(dialogService)
        {
            _workspaceManager = workspaceManager;
            _authService = authService;
            _updateTimer = new Timer();
            _updateTimer.Interval = 5000;
            _updateTimer.Elapsed += OnTimerElapsed;
            _updateTimer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsBusy && _authService.LoggedIn)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        IsBusy = true;

                        var workspaces = await _workspaceManager.GetList();

                        foreach (var workspace in workspaces)
                        {
                            var localWorkspace = Items.FirstOrDefault(x => x.WorkspaceNumber == workspace.WorkspaceNumber);
                            if (localWorkspace != null)
                            {
                                localWorkspace.Wei = workspace.Wei;
                            }
                        }

                        IsBusy = false;
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                });
            }
        }

        protected override void UpdateCommandStates()
        {
            base.UpdateCommandStates();

            CopyToClipboardCommand.NotifyCanExecuteChanged();
        }

        protected override async Task OnAdd()
        {
            await DialogService.ShowDialog(new WorkspaceDetailViewModel(_workspaceManager, DialogService));
        }

        protected override async Task OnEdit()
        {
            await DialogService.ShowDialog(new WorkspaceDetailViewModel(SelectedItem, _workspaceManager, DialogService));
        }

        protected override async Task OnDelete()
        {
            await _workspaceManager.Delete(SelectedItem.WorkspaceNumber);
        }

        protected override async Task<WorkspaceModel[]> OnReload()
        {
            return await _workspaceManager.GetList();
        }

        [RelayCommand(CanExecute = nameof(CanCopyToClipboard))]
        public void CopyToClipboard()
        {
            if (CanCopyToClipboard())
            {
                Clipboard.SetDataObject(SelectedItem.WorkspaceNumber);
            }
        }

        public bool CanCopyToClipboard()
        {
            return !IsBusy && SelectedItem != null;
        }
    }
}
