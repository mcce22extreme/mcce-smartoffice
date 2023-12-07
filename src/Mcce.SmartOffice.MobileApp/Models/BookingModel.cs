using Mcce.SmartOffice.MobileApp.Enums;

namespace Mcce.SmartOffice.MobileApp.Models
{
    public class BookingModel
    {
        public string BookingNumber { get; set; }

        public string BookingTitle { get { return $"{WorkspaceNumber} ({State})"; } }

        public string BookingSubTitle { get { return $"{StartDateTime} - {EndDateTime}"; } }

        public string WorkspaceNumber { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingState State { get; set; }
    }
}
