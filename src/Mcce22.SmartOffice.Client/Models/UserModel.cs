namespace Mcce22.SmartOffice.Client.Models
{
    public class UserModel : IModel
    {
        public string Identifier { get { return UserName; } }

        public bool IsAdmin { get; set; }

        public bool IsEnabled { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullName { get { return $"{FirstName} {LastName} ({UserName})"; } }        
    }
}
