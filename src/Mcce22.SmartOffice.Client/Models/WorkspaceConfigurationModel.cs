namespace Mcce22.SmartOffice.Client.Models
{
    public class WorkspaceConfigurationModel : IModel
    {
        public string Id { get; set; }

        public long DeskHeight { get; set; }

        public string WorkspaceId { get; set; }

        public string UserId { get; set; }

        public string WorkspaceNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string FullUserName { get { return $"{FirstName} {LastName} ({UserName})"; } }
    }
}
