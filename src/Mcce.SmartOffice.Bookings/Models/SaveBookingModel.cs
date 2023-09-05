namespace Mcce.SmartOffice.Bookings.Models
{
    public class SaveBookingModel
    {
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string WorkspaceNumber { get; set; }
    }
}
