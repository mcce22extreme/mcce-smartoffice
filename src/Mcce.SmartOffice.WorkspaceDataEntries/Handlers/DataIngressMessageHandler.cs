using Mcce.SmartOffice.Core.Services;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Mcce.SmartOffice.WorkspaceDataEntries.Generators;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Handlers
{
    public class DataIngressMessageHandler : IMessageHandler
    {
        private const string DATA_INGRESS = "mcce22-smart-office/dataingress";

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IWeiGenerator _weiGenerator;

        public string[] SupportedTopics => new[] { DATA_INGRESS };

        public DataIngressMessageHandler(IServiceScopeFactory serviceScopeFactory, IWeiGenerator weiGenerator)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _weiGenerator = weiGenerator;
        }

        public async Task Handle(string topic, string payload)
        {
            var model = JsonConvert.DeserializeObject<SaveWorkspaceDataEntryModel>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var entry = new WorkspaceDataEntry
            {
                Timestamp = DateTime.UtcNow,
                WorkspaceNumber = model.WorkspaceNumber,
                Temperature = model.Temperature,
                Humidity = model.Humidity,
                Co2Level = model.Co2Level,
            };

            entry.Wei = _weiGenerator.GenerateWei(entry.Temperature, entry.Humidity, entry.Co2Level);

            using var tx = await dbContext.Database.BeginTransactionAsync();

            await dbContext.WorkspaceDataEntries.AddAsync(entry);

            await dbContext.SaveChangesAsync();

            await tx.CommitAsync();
        }
    }
}
