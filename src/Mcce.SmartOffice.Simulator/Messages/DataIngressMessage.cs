namespace Mcce.SmartOffice.Simulator.Messages
{
    public class DataIngressMessage
    {
        public string WorkspaceNumber { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Co2Level { get; set; }
    }
}
