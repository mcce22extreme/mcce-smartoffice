using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
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
        public WorkspaceManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService )
        {
        }

        public Task<WorkspaceModel[]> GetWorkspaces()
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspace";

                var json = await httpClient.GetStringAsync(url);

                var workspaces = JsonConvert.DeserializeObject<WorkspaceModel[]>(json);

                return workspaces;
            });
        }
    }
}
