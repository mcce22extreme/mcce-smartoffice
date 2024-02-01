using Mcce.SmartOffice.Api.Constants;
using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Models;
using Mcce.SmartOffice.Api.Services;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Api.Handlers
{
    public class DataIngressMessageHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_DATAINGRESS };

        public DataIngressMessageHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(string topic, string payload)
        {
            var model = JsonConvert.DeserializeObject<SaveWorkspaceDataEntryModel>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var dataEntryManager = scope.ServiceProvider.GetRequiredService<IWorkspaceDataEntryManager>();

            await dataEntryManager.CreateWorkspaceDataEntry(model.WorkspaceNumber, new SaveWorkspaceDataEntryModel
            {
                Timestamp = DateTime.UtcNow,
                WorkspaceNumber = model.WorkspaceNumber,
                Temperature = model.Temperature,
                Humidity = model.Humidity,
                Co2Level = model.Co2Level,
            });
        }
    }
}
