using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.Bookings.Entities
{
    public class Booking : AuditableEntityBase
    {
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public bool Activated { get; set; }

        public bool InvitationSent { get; set; }

        public string ActivationCode { get; set; }

        public string WorkspaceNumber { get; set; }

        public string UserName { get; set; }
    }
}
