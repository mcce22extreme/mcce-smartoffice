namespace Mcce.SmartOffice.Api.Configs
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string ConnectionString { get; }

        string AppConfigUrl { get; }

        string FrontendUrl { get; }

        string StoragePath { get; set; }

        AuthConfig AuthConfig { get; }

        MqttConfig MqttConfig { get; }

        CorsConfig CorsConfig { get; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string ConnectionString { get; set; }

        public string StoragePath { get; set; }

        public string AppConfigUrl { get; set; }

        public string FrontendUrl { get; set; }

        public AuthConfig AuthConfig { get; set; }

        public MqttConfig MqttConfig { get; set; }

        public CorsConfig CorsConfig { get; set; }
    }
}
