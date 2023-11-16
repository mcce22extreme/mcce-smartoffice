namespace Mcce.SmartOffice.Bookings.Messages
{
    public class BookingActivatedMessage
    {
        public string BookingNumber { get; }

        public string WorkspaceNumber { get; }

        public string UserName { get; }

        public BookingActivatedMessage(string bookingNumber, string workspaceNumber, string userName)
        {
            BookingNumber = bookingNumber;
            WorkspaceNumber = workspaceNumber;
            UserName = userName;
        }
    }
}
