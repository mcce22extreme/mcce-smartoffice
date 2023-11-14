using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public class WorkspaceConfigurationListViewModel : ListViewModelBase<WorkspaceConfigurationModel>
    {
        private readonly IWorkspaceConfigurationManager _workspaceConfigurationManager;
        private readonly IWorkspaceManager _workspaceManager;

        public WorkspaceConfigurationListViewModel(
            IWorkspaceConfigurationManager workspaceConfigurationManager,
            IWorkspaceManager workspaceManager,
            IDialogService dialogService)
            : base(dialogService)
        {
            _workspaceConfigurationManager = workspaceConfigurationManager;
            _workspaceManager = workspaceManager;
        }

        protected override async Task OnAdd()
        {
            await DialogService.ShowDialog(new WorkspaceConfigurationDetailViewModel(_workspaceConfigurationManager, _workspaceManager, DialogService));
        }

        protected override async Task OnEdit()
        {
            await DialogService.ShowDialog(new WorkspaceConfigurationDetailViewModel(SelectedItem, _workspaceConfigurationManager, _workspaceManager, DialogService));
        }

        protected override async Task OnDelete()
        {
            await _workspaceConfigurationManager.Delete(SelectedItem.WorkspaceNumber);
        }

        protected override async Task<WorkspaceConfigurationModel[]> OnReload()
        {
            return await _workspaceConfigurationManager.GetList();
        }
    }
}
