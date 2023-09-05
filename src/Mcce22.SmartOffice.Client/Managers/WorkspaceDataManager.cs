using System.Net.Http;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IWorkspaceDataManager
    {
        Task<WorkspaceDataModel[]> GetList(string workspaceId);

        Task<WorkspaceDataModel> Save(WorkspaceDataModel model);

        Task DeleteAll();
    }

    public class WorkspaceDataManager : ManagerBase<WorkspaceDataModel>, IWorkspaceDataManager
    {
        public WorkspaceDataManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspacedata/", httpClient)
        {
        }

        public async Task<WorkspaceDataModel[]> GetList(string workspaceId)
        {
            var json = await HttpClient.GetStringAsync($"{BaseUrl}?workspaceid={workspaceId}");

            var entries = JsonConvert.DeserializeObject<WorkspaceDataModel[]>(json);

            return entries;
        }

        public async Task DeleteAll()
        {
            var url = $"{BaseUrl}/deleteall";

            await HttpClient.DeleteAsync(url);
        }
    }
}
