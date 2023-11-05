using System;
using Mcce22.SmartOffice.Client.Enums;

namespace Mcce22.SmartOffice.Client.Models
{
    public class BookingModel : ModelBase
    {
        public string BookingNumber { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingState State { get; set; }

        public string WorkspaceId { get; set; }

        public string UserId { get; set; }

        public string WorkspaceNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullUserName { get { return $"{FirstName} {LastName} ({UserName})"; } }
    }
}
