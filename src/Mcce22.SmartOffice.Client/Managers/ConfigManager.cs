using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IConfigManager
    {
        Task<MqttConfigModel> GetMqttConfig();

        Task SaveMqttConfig(MqttConfigModel model);
    }

    public class ConfigManager : IConfigManager
    {
        public async Task<MqttConfigModel> GetMqttConfig()
        {
            await Task.Delay(3000);

            return new MqttConfigModel
            {
                HostName = "localhost",
            };
        }

        public Task SaveMqttConfig(MqttConfigModel model)
        {
            return Task.CompletedTask;
        }
    }
}
