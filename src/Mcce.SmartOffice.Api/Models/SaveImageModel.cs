using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Api.Models
{
    public class SaveImageModel
    {
        [FromBody]
        public Stream Stream { get; set; }

        [FromQuery]
        public string FileName { get; set; }
    }
}
