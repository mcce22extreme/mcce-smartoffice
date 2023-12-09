using System.Net.Http.Json;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;

namespace Mcce.SmartOffice.AdminApp.Managers
{
    public interface IWorkspaceDataManager
    {
        Task<WorkspaceDataModel[]> GetWorkspaceData(string workspaceNumber, DateTime startDate, DateTime endDate);

        Task DeleteWorkspaceData(string workspaceNumber);
    }

    public class WorkspaceDataManager : ManagerBase, IWorkspaceDataManager
    {
        public WorkspaceDataManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService)
        {
        }

        public Task<WorkspaceDataModel[]> GetWorkspaceData(string workspaceNumber, DateTime startDate, DateTime endDate)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspacedataentry/{workspaceNumber}?startDate={startDate:s}&endDate={endDate:s}";

                var entries = await httpClient.GetFromJsonAsync<WorkspaceDataModel[]>(url);

                return entries;
            });
        }

        public async Task DeleteWorkspaceData(string workspaceNumber)
        {
            await ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspacedataentry/{workspaceNumber}";

                var response = await httpClient.DeleteAsync(url);

                return Task.CompletedTask;
            });
        }
    }
}
