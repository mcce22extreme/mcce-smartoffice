using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Bookings.Enums;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.Bookings.Entities
{
    public class Booking : AuditableEntityBase
    {
        [Required]
        public string BookingNumber { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        public string WorkspaceNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public BookingState State { get; set; }
    }
}
