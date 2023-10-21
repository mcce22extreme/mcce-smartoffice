using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.UserImages.Entities
{
    public class UserImage : EntityBase
    {
        [Key]
        public string ImageKey { get; set; }

        public string UserName { get; set; }        
    }
}
