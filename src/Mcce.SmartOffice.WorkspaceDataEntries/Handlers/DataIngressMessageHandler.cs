using Mcce.SmartOffice.Common.Constants;
using Mcce.SmartOffice.Common.Services;
using Mcce.SmartOffice.WorkspaceDataEntries.Managers;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Handlers
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
