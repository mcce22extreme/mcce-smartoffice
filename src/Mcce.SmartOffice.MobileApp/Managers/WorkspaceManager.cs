using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetWorkspaces();
    }

    public class WorkspaceManager : ManagerBase, IWorkspaceManager
    {
        public WorkspaceManager(IAppConfig appConfig, IHttpClientFactory httpClientFactory, ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<WorkspaceModel[]> GetWorkspaces()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspace";

            var json = await httpClient.GetStringAsync(url);

            var workspaces = JsonConvert.DeserializeObject<WorkspaceModel[]>(json);

            return workspaces;
        }
    }
}
