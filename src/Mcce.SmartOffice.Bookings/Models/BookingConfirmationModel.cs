namespace Mcce.SmartOffice.Bookings.Models
{
    public class BookingConfirmationModel
    {
        public string BookingNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string WorkspaceNumber { get; set; }
    }
}
