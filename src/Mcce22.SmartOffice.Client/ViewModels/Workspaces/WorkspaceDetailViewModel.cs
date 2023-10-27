using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class WorkspaceDetailViewModel : DialogViewModelBase
    {
        private readonly IWorkspaceManager _workspaceManager;

        [ObservableProperty]
        private string _workspaceNumber;

        [ObservableProperty]
        private string _roomNumber;

        [ObservableProperty]
        private int _top;

        [ObservableProperty]
        private int _left;

        [ObservableProperty]
        private int _width;

        [ObservableProperty]
        private int _height;
        
        public WorkspaceDetailViewModel(IWorkspaceManager workspaceManager, IDialogService dialogService)
            : base(dialogService)
        {
            Title = "Create workspace";
            _workspaceManager = workspaceManager;
        }

        public WorkspaceDetailViewModel(WorkspaceModel model, IWorkspaceManager workspaceManager, IDialogService dialogService)
            : this(workspaceManager, dialogService)
        {
            Title = "Edit workspace";

            WorkspaceNumber = model.WorkspaceNumber;
            RoomNumber = model.RoomNumber;
            Top = model.Top;
            Left = model.Left;
            Width = model.Width;
            Height = model.Height;
        }

        protected override async Task OnSave()
        {
            await _workspaceManager.Save(new WorkspaceModel
            {
                WorkspaceNumber = WorkspaceNumber,
                RoomNumber = RoomNumber,
                Top = Top,
                Left = Left,
                Width = Width,
                Height = Height,
            });
        }
    }
}
