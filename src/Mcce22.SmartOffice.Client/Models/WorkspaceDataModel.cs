using System;

namespace Mcce22.SmartOffice.Client.Models
{
    public class WorkspaceDataModel : ModelBase
    {
        public string WorkspaceId { get; set; }

        public string WorkspaceNumber { get; set; }

        public string RoomNumber { get; set; }

        public DateTime Timestamp { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Co2Level { get; set; }

        public int Wei { get; set; }        
    }
}
