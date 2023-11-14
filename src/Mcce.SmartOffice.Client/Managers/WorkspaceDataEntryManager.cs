using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce.SmartOffice.Client.Managers
{
    public interface IWorkspaceDataEntryManager
    {
        Task<WorkspaceDataModel[]> GetList(string workspaceNumber, DateTime? startDate, DateTime? endDate);

        Task<WorkspaceDataModel> Save(WorkspaceDataModel model);

        Task Delete(string workspaceNumber);
    }

    public class WorkspaceDataEntryManager : ManagerBase<WorkspaceDataModel>, IWorkspaceDataEntryManager
    {
        public WorkspaceDataEntryManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspacedataentry", httpClient)
        {
        }

        public async Task<WorkspaceDataModel[]> GetList(string workspaceNumber, DateTime? startDate, DateTime? endDate)
        {           
            var queryString = string.Empty;

            if(startDate.HasValue)
            {
                queryString = $"startDate={startDate:s}";
            }

            if(endDate.HasValue)
            {
                if(!string.IsNullOrEmpty(queryString))
                {
                    queryString += "&";
                }

                queryString += $"endDate={endDate:s}";
            }

            var url = $"{BaseUrl}/{workspaceNumber}" + (string.IsNullOrEmpty(queryString) ? string.Empty : $"?{queryString}");

            var json = await HttpClient.GetStringAsync(url);

            var entries = JsonConvert.DeserializeObject<WorkspaceDataModel[]>(json);

            return entries;
        }

        public override async Task<WorkspaceDataModel> Save(WorkspaceDataModel model)
        {
            var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<WorkspaceDataModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
