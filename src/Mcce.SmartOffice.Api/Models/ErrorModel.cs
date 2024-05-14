using System.Net;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Api.Models
{
    public class ErrorModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
