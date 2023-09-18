namespace Mcce.SmartOffice.WorkspaceDataEntries.Models
{
    public class WorkspaceDataEntryModel
    {
        public string EntryId { get; set; }

        public string WorkspaceNumber { get; set; }

        public DateTime Timestamp { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Co2Level { get; set; }

        public int Wei { get; set; }
    }
}
