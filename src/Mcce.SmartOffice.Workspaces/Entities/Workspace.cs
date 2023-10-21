using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.Workspaces.Entities
{
    public class Workspace : AuditableEntityBase
    {
        [Key]
        public string WorkspaceNumber { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Wei { get; set; }
    }
}
