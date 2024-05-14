using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.Api.Entities
{
    public class WorkspaceDataEntry : EntityBase
    {
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int Wei { get; set; }

        public float Temperature { get; set; }

        public float Co2Level { get; set; }

        public float Humidity { get; set; }

        [Required]
        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; }
    }
}
