using System.Net.Http.Json;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
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
            ISecureStorage secureStorage)
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

        public async Task CreateWorkspaces(WorkspaceModel model)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspace";

            var response = await httpClient.PostAsJsonAsync(url, model);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteWorkspaces(string workspaceNumber)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspace/{workspaceNumber}";

            var response = await httpClient.DeleteAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}
