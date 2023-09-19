using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Simulator.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetWorkspaces();
    }

    public class WorkspaceManager : IWorkspaceManager
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public WorkspaceManager(string baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        public async Task<WorkspaceModel[]> GetWorkspaces()
        {
            var json = await _httpClient.GetStringAsync($"{_baseUrl}/workspace");

            var workspaces = JsonConvert.DeserializeObject<WorkspaceModel[]>(json);

            return workspaces;

        }
    }
}
