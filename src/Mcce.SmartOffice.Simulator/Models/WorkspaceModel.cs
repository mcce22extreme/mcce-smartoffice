using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce.SmartOffice.Simulator
{
    public partial class WorkspaceModel : ObservableObject
    {
        public string Id { get; set; }

        public string WorkspaceNumber { get; set; }
    }
}
