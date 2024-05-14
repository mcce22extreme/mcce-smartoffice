using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.Api.Entities
{
    public class UserImage : EntityBase
    {
        [Required]
        public string ImageKey { get; set; }

        [Required]
        public string ThumbnailImageKey { get; set; }

        [Required]
        public string UserName { get; set; }        
    }
}
