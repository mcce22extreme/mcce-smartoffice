using System.Buffers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class WorkspaceConfigurationDetailViewModel : DialogViewModelBase
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;
        private readonly IWorkspaceManager _workspaceManager;

        private readonly string _workspaceNumber;

        [ObservableProperty]
        private long _deskHeight = 70;

        [ObservableProperty]
        private ObservableCollection<WorkspaceModel> _workspaces = new ObservableCollection<WorkspaceModel>();

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;

        public WorkspaceConfigurationDetailViewModel(
            IWorkspaceConfigurationManager workspaceConfigurationManager,
            IWorkspaceManager workspaceManager,
            IDialogService dialogService)
            : base(dialogService)
        {
            Title = "Create workspace configuration";

            _workspaceConfigurationManager = workspaceConfigurationManager;
            _workspaceManager = workspaceManager;
        }

        public WorkspaceConfigurationDetailViewModel(
            WorkspaceConfigurationModel model,
            IWorkspaceConfigurationManager workspaceConfigurationManager,
            IWorkspaceManager workspaceManager,
            IDialogService dialogService)
            : this(workspaceConfigurationManager, workspaceManager, dialogService)
        {
            _workspaceNumber = model.WorkspaceNumber;

            Title = "Edit workspace configuration";
            DeskHeight = model.DeskHeight;            
        }

        public override async void Load()
        {
            try
            {
                IsBusy = true;

                var workspaces = await _workspaceManager.GetList();
                var configurations = await _workspaceConfigurationManager.GetList();
                var lookup = configurations.ToLookup(x => x.WorkspaceNumber);

                Workspaces = new ObservableCollection<WorkspaceModel>(workspaces.Where(x => !lookup.Contains(x.WorkspaceNumber)));

                SelectedWorkspace =  string.IsNullOrEmpty(_workspaceNumber) ? null : workspaces.FirstOrDefault(x => x.WorkspaceNumber == _workspaceNumber);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected override bool CanSave()
        {
            return !IsBusy && SelectedWorkspace != null;
        }

        protected override async Task OnSave()
        {
            await _workspaceConfigurationManager.Save(new WorkspaceConfigurationModel
            {
                DeskHeight = DeskHeight,
                WorkspaceNumber = SelectedWorkspace.WorkspaceNumber
            });
        }
    }
}
