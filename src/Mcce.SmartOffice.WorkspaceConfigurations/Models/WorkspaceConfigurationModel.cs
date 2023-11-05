using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.WorkspaceConfigurations.Models
{
    public class WorkspaceConfigurationModel : AuditableModelBase
    {
        public long DeskHeight { get; set; }

        public string WorkspaceNumber { get; set; }
    }
}
