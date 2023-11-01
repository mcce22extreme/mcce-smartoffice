﻿using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.Managers
{
    public interface IWorkspaceConfigurationManager
    {
        Task<WorkspaceConfigurationModel[]> GetList();

        Task<WorkspaceConfigurationModel> Create(WorkspaceConfigurationModel user);

        Task<WorkspaceConfigurationModel> Update(WorkspaceConfigurationModel user);

        Task Delete(string workspaceNumber);
    }

    public class WorkspaceConfigurationManager : ManagerBase<WorkspaceConfigurationModel>, IWorkspaceConfigurationManager
    {
        public WorkspaceConfigurationManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspaceconfiguration", httpClient)
        {
        }

        public override async Task<WorkspaceConfigurationModel> Create(WorkspaceConfigurationModel model)
        {
            var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<WorkspaceConfigurationModel>(await response.Content.ReadAsStringAsync());
        }

        public override async Task<WorkspaceConfigurationModel> Update(WorkspaceConfigurationModel model)
        {
            var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{model.WorkspaceNumber}", model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<WorkspaceConfigurationModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
