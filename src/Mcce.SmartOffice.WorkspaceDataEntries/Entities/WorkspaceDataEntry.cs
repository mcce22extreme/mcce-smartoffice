using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Entities
{
    public class WorkspaceDataEntry
    {
        [Key]
        public string EntryId { get; set; }

        [Required]
        public string WorkspaceNumber { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int Wei { get; set; }

        public double Temperature { get; set; }

        public double Co2Level { get; set; }

        public double Humidity { get; set; }
    }
}
