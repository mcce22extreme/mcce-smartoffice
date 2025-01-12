﻿using System.Net.Http;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Models;

namespace Mcce.SmartOffice.Client.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetList();

        Task<WorkspaceModel> Save(WorkspaceModel workspace);

        Task Delete(string workspaceId);
    }

    public class WorkspaceManager : ManagerBase<WorkspaceModel>, IWorkspaceManager
    {
        public WorkspaceManager(IAppConfig appConfig, HttpClient httpClient)
            : base($"{appConfig.BaseAddress}/workspace", httpClient)
        {
        }
    }
}
