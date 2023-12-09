namespace Mcce.SmartOffice.AdminApp.Models
{
    public class WorkspaceDataModel
    {
        public int Id { get; set; }

        public string WorkspaceNumber { get; set; }

        public string RoomNumber { get; set; }

        public DateTime Timestamp { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Co2Level { get; set; }

        public int Wei { get; set; }
    }
}
