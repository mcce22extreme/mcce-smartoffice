using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Bookings.Models
{
    public class BookingModel : AuditableModelBase
    {
        public string BookingNumber { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string WorkspaceNumber { get; set; }

        public bool Activated { get; set; }

        public bool InvitationSent { get; set; }
    }
}
