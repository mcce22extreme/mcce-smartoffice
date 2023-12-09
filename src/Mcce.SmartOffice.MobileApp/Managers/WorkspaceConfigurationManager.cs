using System.Net.Http.Json;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.MobileApp.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.MobileApp.Managers
{
    public interface IWorkspaceConfigurationManager
    {
        Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations();

        Task SaveWorkspaceConfigurations(string workspaceNumber, int deskHeight);

        Task DeleteBooking(string workspaceNumber);
    }

    public class WorkspaceConfigurationManager : ManagerBase, IWorkspaceConfigurationManager
    {
        public WorkspaceConfigurationManager(
            IAppConfig appConfig,
            IHttpClientFactory httpClientFactory,
            ISecureStorage secureStorage,
            IAuthService authService)
            : base(appConfig, httpClientFactory, secureStorage, authService )
        {
        }

        public Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations()
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspaceconfiguration";

                var json = await httpClient.GetStringAsync(url);

                var configurations = JsonConvert.DeserializeObject<WorkspaceConfigurationModel[]>(json);

                return configurations;
            });
        }

        public Task SaveWorkspaceConfigurations(string workspaceNumber, int deskHeight)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspaceconfiguration/{workspaceNumber}";

                var response = await httpClient.PostAsJsonAsync(url, new
                {
                    WorkspaceNumber = workspaceNumber,
                    DeskHeight = deskHeight,
                });

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }

        public Task DeleteBooking(string workspaceNumber)
        {
            return ExecuteRequest(async httpClient =>
            {
                var url = $"{AppConfig.BaseAddress}workspaceconfiguration/{workspaceNumber}";

                var response = await httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                return Task.CompletedTask;
            });
        }
    }
}
