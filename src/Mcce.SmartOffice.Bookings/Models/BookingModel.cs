using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Bookings.Models
{
    public class BookingModel : AuditableModelBase
    {
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string UserName { get; set; }

        public string WorkspaceNumber { get; set; }

        public bool Activated { get; set; }

        public bool InvitationSent { get; set; }
    }
}
