using System.Net.Http.Json;
using Mcce.SmartOffice.App;
using Mcce.SmartOffice.App.Managers;
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
            ISecureStorage secureStorage)
            : base(appConfig, httpClientFactory, secureStorage)
        {
        }

        public async Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations()
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspaceconfiguration";

            var json = await httpClient.GetStringAsync(url);

            var configurations = JsonConvert.DeserializeObject<WorkspaceConfigurationModel[]>(json);

            return configurations;
        }

        public async Task SaveWorkspaceConfigurations(string workspaceNumber, int deskHeight)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspaceconfiguration/{workspaceNumber}";

            var response = await httpClient.PostAsJsonAsync(url, new
            {
                WorkspaceNumber = workspaceNumber,
                DeskHeight = deskHeight,
            });

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBooking(string workspaceNumber)
        {
            using var httpClient = await CreateHttpClient();

            var url = $"{AppConfig.BaseAddress}workspaceconfiguration/{workspaceNumber}";

            var response = await httpClient.DeleteAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}
