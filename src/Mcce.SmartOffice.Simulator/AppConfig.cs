using Mcce.SmartOffice.Common.Configs;

namespace Mcce.SmartOffice.Simulator
{
    public interface IAppConfig
    {
        string BaseAddress { get; }

        string AuthEndpoint { get; }

        string ClientSecret { get; }

        string ClientId { get; }

        MqttConfig MqttConfig { get; }
    }

    public class AppConfig : IAppConfig
    {
        public string BaseAddress { get; set; }

        public string AuthEndpoint { get; set; }

        public string ClientSecret { get; set; }

        public string ClientId { get; set; }

        public MqttConfig MqttConfig { get; set; }
    }
}
