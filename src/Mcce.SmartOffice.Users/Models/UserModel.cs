using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Users.Models
{
    public class UserModel : AuditableModelBase
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsAdmin { get; set; }
    }
}
