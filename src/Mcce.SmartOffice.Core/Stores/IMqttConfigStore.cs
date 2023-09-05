using Mcce.SmartOffice.Core.Configs;

namespace Mcce.SmartOffice.Core.Stores
{
    public interface IMqttConfigStore
    {
        Task<MqttConfig> GetMqttConfig();

        Task<MqttConfig> SaveMqttConfig();
    }
}
