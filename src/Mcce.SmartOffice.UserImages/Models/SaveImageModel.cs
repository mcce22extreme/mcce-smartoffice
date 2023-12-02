using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.UserImages.Models
{
    public class SaveImageModel
    {
        [FromBody]
        public Stream Stream { get; set; }

        [FromQuery]
        public string FileName { get; set; }
    }
}
