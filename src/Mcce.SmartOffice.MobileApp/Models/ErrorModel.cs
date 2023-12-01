using System.Net;

namespace Mcce.SmartOffice.MobileApp.Models
{
    public class ErrorModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
