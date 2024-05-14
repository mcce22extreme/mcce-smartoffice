using Mcce.SmartOffice.Api.Constants;
using Mcce.SmartOffice.Api.Managers;
using Mcce.SmartOffice.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceManager _workspaceManager;

        public WorkspaceController(IWorkspaceManager workspaceManager)
        {
            _workspaceManager = workspaceManager;
        }

        [HttpGet]
        public async Task<WorkspaceModel[]> GetWorkspaces()
        {
            return await _workspaceManager.GetWorkspaces();
        }

        [HttpGet("{workspaceNumber}")]
        public async Task<WorkspaceModel> GetWorkspace(string workspaceNumber)
        {
            return await _workspaceManager.GetWorkspace(workspaceNumber);
        }

        [HttpPost]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task<WorkspaceModel> CreateWorkspace([FromBody] SaveWorkspaceModel model)
        {
            return await _workspaceManager.CreateWorkspace(model);
        }

        [HttpPut("{workspaceNumber}")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, [FromBody] SaveWorkspaceModel model)
        {
            return await _workspaceManager.UpdateWorkspace(workspaceNumber, model);
        }

        [HttpDelete("{workspaceNumber}")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task DeleteWorkspace(string workspaceNumber)
        {
            await _workspaceManager.DeleteWorkspace(workspaceNumber);
        }
    }
}
