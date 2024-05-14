namespace Mcce.SmartOffice.Api.Models
{
    public class WorkspaceDataEntryModel : ModelBase
    {
        public string WorkspaceNumber { get; set; }

        public DateTime Timestamp { get; set; }

        public float Temperature { get; set; }

        public float Humidity { get; set; }

        public float Co2Level { get; set; }

        public int Wei { get; set; }
    }
}
