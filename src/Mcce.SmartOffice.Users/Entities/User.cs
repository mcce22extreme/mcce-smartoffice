using System.ComponentModel.DataAnnotations;
using Mcce.SmartOffice.Core.Entities;

namespace Mcce.SmartOffice.Users.Entities
{
    public class User : AuditableEntityBase
    {
        public bool IsEnabled { get; set; }

        public bool IsAdmin { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
