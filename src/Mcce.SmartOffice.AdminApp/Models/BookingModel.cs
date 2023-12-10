using Mcce.SmartOffice.AdminApp.Enums;

namespace Mcce.SmartOffice.AdminApp.Models
{
    public class BookingModel
    {
        public string BookingNumber { get; set; }

        public string BookingTitle { get { return $"{WorkspaceNumber} ({State})"; } }

        public string BookingSubTitle { get { return $"{UserName}, {StartDateTime} - {EndDateTime}"; } }

        public string WorkspaceNumber { get; set; }

        public string UserName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingState State { get; set; }
    }
}
