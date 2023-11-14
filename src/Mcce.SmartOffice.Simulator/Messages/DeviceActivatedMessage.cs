namespace Mcce.SmartOffice.Simulator.Messages
{
    public class DeviceActivatedMessage
    {
        public string WorkspaceNumber { get; set; }

        public string UserId { get; set; }

        public string BookingId { get; set; }

        public double DeskHeight { get; set; }

        public string[] UserImageUrls { get; set; }
    }
}
