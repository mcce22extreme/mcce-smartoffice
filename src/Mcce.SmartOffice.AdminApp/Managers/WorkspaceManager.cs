using System.Net.Http.Json;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.AdminApp.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetWorkspaces();

        Task CreateWorkspaces(WorkspaceModel model);

        Task DeleteWorkspaces(string workspaceNumber);
    }

    public class WorkspaceManager : ManagerBase, IWorkspaceManager
    {
        public WorkspaceManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService)
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

        public Task CreateWorkspaces(WorkspaceModel model)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspace";

                var response = await httpClient.PostAsJsonAsync(url, model);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }

        public  Task DeleteWorkspaces(string workspaceNumber)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspace/{workspaceNumber}";

                var response = await httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }
    }
}
