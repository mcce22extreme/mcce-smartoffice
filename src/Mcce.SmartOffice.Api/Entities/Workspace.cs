using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.Api.Entities
{
    public class Workspace : AuditableEntityBase
    {
        [Required]
        public string WorkspaceNumber { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Wei { get; set; }

        public List<WorkspaceDataEntry> DataEntries { get; set; }

        public List<WorkspaceConfiguration> Configurations { get; set; }
    }
}
