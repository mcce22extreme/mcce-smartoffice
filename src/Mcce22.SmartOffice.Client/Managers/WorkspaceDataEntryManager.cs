using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IWorkspaceDataEntryManager
    {
        Task<WorkspaceDataModel[]> GetList(string workspaceNumber);

        Task<WorkspaceDataModel> Create(WorkspaceDataModel model);

        Task Delete(string workspaceNumber);
    }

    public class WorkspaceDataEntryManager : ManagerBase<WorkspaceDataModel>, IWorkspaceDataEntryManager
    {
        public WorkspaceDataEntryManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspacedataentry", httpClient)
        {
        }

        public async Task<WorkspaceDataModel[]> GetList(string workspaceNumber)
        {
            var json = await HttpClient.GetStringAsync($"{BaseUrl}/{workspaceNumber}");

            var entries = JsonConvert.DeserializeObject<WorkspaceDataModel[]>(json);

            return entries;
        }

        public override async Task<WorkspaceDataModel> Create(WorkspaceDataModel model)
        {
            var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<WorkspaceDataModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
