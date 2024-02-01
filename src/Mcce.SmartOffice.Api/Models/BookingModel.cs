using Mcce.SmartOffice.Api.Enums;

namespace Mcce.SmartOffice.Api.Models
{
    public class BookingModel : AuditableModelBase
    {
        public string BookingNumber { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string WorkspaceNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public BookingState State { get; set; }
    }
}
