namespace Mcce22.SmartOffice.Client.Models
{
    public class WorkspaceConfigurationModel : IModel
    {
        public string Identifier { get { return ConfigurationNumber; } }

        public string ConfigurationNumber { get; set; }

        public long DeskHeight { get; set; }

        public string WorkspaceNumber { get; set; }
    }
}
