using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.Common.Services;
using Mcce.SmartOffice.DigitalFrameApp.Models;

namespace Mcce.SmartOffice.DigitalFrameApp.Managers
{
    public interface IWorkspaceDataManager
    {
        Task<WorkspaceDataModel> SendWorkspaceData();
    }

    public class WorkspaceDataManager : ManagerBase, IWorkspaceDataManager
    {
        private const string TOPIC_DATA_INGRESS = "mcce-smartoffice/dataingress";

        private const double DEFAULT_TEMPERATURE = 20;

        private const double DEFAULT_HUMIDITY = 20;

        private const double DEFAULT_CO2LEVEL = 700;

        private static readonly Random Random = new Random();

        private readonly IMessageService _messageService;

        public WorkspaceDataManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IMessageService messageService)
            : base(appConfig, httpClientFactory, secureStorage)
        {
            _messageService = messageService;
        }

        public async Task<WorkspaceDataModel> SendWorkspaceData()
        {
            var model = new WorkspaceDataModel
            {
                WorkspaceNumber = AppConfig.WorkspaceNumber,
                Temperature = Random.Next((int)DEFAULT_TEMPERATURE - 2, (int)DEFAULT_TEMPERATURE + 2) + Random.NextDouble(),
                Humidity = Random.Next((int)DEFAULT_HUMIDITY - 5, (int)DEFAULT_HUMIDITY + 5) + Random.NextDouble(),
                Co2Level = Random.Next((int)DEFAULT_CO2LEVEL - 10, (int)DEFAULT_CO2LEVEL + 15) + Random.NextDouble()
            };

            await _messageService.Publish(TOPIC_DATA_INGRESS, model);

            return model;
        }
    }
}
