using Mcce.SmartOffice.Common.Configs;

namespace Mcce.SmartOffice.App
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string WorkspaceNumber { get; set; }

        AuthConfig AuthConfig { get; set; }

        MqttConfig MqttConfig { get; set; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string WorkspaceNumber { get; set; }

        public AuthConfig AuthConfig { get;set;}

        public MqttConfig MqttConfig { get; set; }
    }

    public class AuthConfig
    {
        public string AuthEndpoint { get; set; }

        public string AuthRedirectUri { get; set; }
    }
}
