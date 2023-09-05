using Mcce.SmartOffice.Core.Configs;

namespace Mcce.SmartOffice.Core.Providers
{
    public interface IMqttConfigProvider
    {
        Task<MqttConfig> GetMqttConfig();
    }
}
