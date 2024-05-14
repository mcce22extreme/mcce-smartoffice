using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.Api.Entities
{
    public class WorkspaceConfiguration : AuditableEntityBase
    {
        [Required]
        public long DeskHeight { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; }
    }
}
