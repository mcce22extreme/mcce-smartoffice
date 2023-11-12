using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Client.Managers
{
    public interface IWorkspaceConfigurationManager
    {
        Task<WorkspaceConfigurationModel[]> GetList();

        Task<WorkspaceConfigurationModel> Save(WorkspaceConfigurationModel user);

        Task Delete(string workspaceNumber);
    }

    public class WorkspaceConfigurationManager : ManagerBase<WorkspaceConfigurationModel>, IWorkspaceConfigurationManager
    {
        public WorkspaceConfigurationManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspaceconfiguration", httpClient)
        {
        }

        public override async Task<WorkspaceConfigurationModel> Save(WorkspaceConfigurationModel model)
        {
            HttpResponseMessage response;

            if (model.Id == 0)
            {
                response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);
            }
            else
            {
                response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);
            }

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<WorkspaceConfigurationModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
