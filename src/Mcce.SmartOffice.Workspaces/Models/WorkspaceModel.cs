using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Workspaces.Models
{
    public class WorkspaceModel : AuditableModelBase
    {
        public string WorkspaceNumber { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Wei { get; set; }
    }
}
