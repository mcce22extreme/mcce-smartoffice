namespace Mcce.SmartOffice.Core.Configs
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AppConfigUrl { get; }

        string FrontendUrl { get; }

        AuthConfig AuthConfig { get; }

        DbConfig DbConfig {  get; }

        MqttConfig MqttConfig { get; }

        CorsConfig CorsConfig { get; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AppConfigUrl { get; set; }

        public string FrontendUrl { get; set; }

        public AuthConfig AuthConfig { get; set; }

        public DbConfig DbConfig { get; set; }

        public MqttConfig MqttConfig { get; set; }

        public CorsConfig CorsConfig { get; set; }
    }
}
