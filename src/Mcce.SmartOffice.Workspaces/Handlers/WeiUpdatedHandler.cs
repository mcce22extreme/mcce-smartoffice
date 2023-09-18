using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Services;
using Mcce.SmartOffice.Workspaces.Managers;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Workspaces.Handlers
{
    public class WeiUpdatedHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public string[] SupportedTopics => new[] { MessageTopics.TOPIC_WEI_UPDATED };

        public WeiUpdatedHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(string topic, string payload)
        {
            var weiInfo = JsonConvert.DeserializeObject<WeiInfo>(payload);

            var scope = _serviceScopeFactory.CreateScope();
            var workspaceManager = scope.ServiceProvider.GetRequiredService<IWorkspaceManager>();

            await workspaceManager.UpdateWorkspaceWei(weiInfo.WorkspaceNumber, weiInfo.Wei);
        }

        private class WeiInfo
        {
            public string WorkspaceNumber { get; set; }

            public int Wei { get; set; }
        }
    }
}
