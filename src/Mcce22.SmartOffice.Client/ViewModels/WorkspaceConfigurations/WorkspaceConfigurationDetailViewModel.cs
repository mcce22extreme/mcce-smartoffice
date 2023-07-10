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
        private readonly IWorkspaceConfigurationManager _userWorkspaceManager;
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUserManager _userManager;

        private readonly string _workspaceId;
        private readonly string _userId;

        [ObservableProperty]
        private long _deskHeight = 70;

        [ObservableProperty]
        private ObservableCollection<WorkspaceModel> _workspaces = new ObservableCollection<WorkspaceModel>();

        [ObservableProperty]
        private ObservableCollection<UserModel> _users = new ObservableCollection<UserModel>();

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;

        [ObservableProperty]
        private UserModel _selectedUser;

        public string UserWorkspaceId { get; }

        public WorkspaceConfigurationDetailViewModel(
            IWorkspaceConfigurationManager userWorkspaceManager,
            IWorkspaceManager workspaceManager,
            IUserManager userManager,
            IDialogService dialogService)
            : base(dialogService)
        {
            Title = "Create workspace configuration";

            _userWorkspaceManager = userWorkspaceManager;
            _workspaceManager = workspaceManager;
            _userManager = userManager;
        }

        public WorkspaceConfigurationDetailViewModel(
            WorkspaceConfigurationModel model,
            IWorkspaceConfigurationManager userWorkspaceManager,
            IWorkspaceManager workspaceManager,
            IUserManager userManager,
            IDialogService dialogService)
            : this(userWorkspaceManager, workspaceManager, userManager, dialogService)
        {
            Title = "Edit workspace configuration";

            UserWorkspaceId = model.Id;
            DeskHeight = model.DeskHeight;

            _workspaceId = model.WorkspaceId;
            _userId = model.UserId;
        }

        public override async void Load()
        {
            try
            {
                IsBusy = true;

                var workspaces = await _workspaceManager.GetList();
                var users = await _userManager.GetList();

                Workspaces = new ObservableCollection<WorkspaceModel>(workspaces);
                Users = new ObservableCollection<UserModel>(users);

                SelectedWorkspace = string.IsNullOrEmpty(_workspaceId) ? null : workspaces.FirstOrDefault(x => x.Id == _workspaceId);
                SelectedUser = string.IsNullOrEmpty(_userId) ? null : users.FirstOrDefault(x => x.Id == _userId);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected override bool CanSave()
        {
            return !IsBusy && SelectedWorkspace != null && SelectedUser != null;
        }

        protected override async Task OnSave()
        {
            await _userWorkspaceManager.Save(new WorkspaceConfigurationModel
            {
                Id = UserWorkspaceId,
                DeskHeight = DeskHeight,
                WorkspaceId = SelectedWorkspace.Id,
                UserId = SelectedUser.Id
            });
        }
    }
}
