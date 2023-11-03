using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.UserImages.Entities
{
    public class UserImage : EntityBase
    {
        [Required]
        public string ImageKey { get; set; }

        [Required]
        public string UserName { get; set; }        
    }
}
