using Mcce.SmartOffice.WorkspaceConfigurations.Managers;
using Mcce.SmartOffice.WorkspaceConfigurations.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.WorkspaceConfigurations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkspaceConfigurationController : ControllerBase
    {
        private readonly IWorkspaceConfigurationManager _configurationManager;

        public WorkspaceConfigurationController(IWorkspaceConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        [HttpGet]
        public async Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations()
        {
            return await _configurationManager.GetWorkspaceConfigurations();
        }

        [HttpGet("{workspaceNumber}")]
        public async Task<WorkspaceConfigurationModel> GetWorkspaceConfiguration(string workspaceNumber)
        {
            return await _configurationManager.GetWorkspaceConfiguration(workspaceNumber);
        }

        [HttpPost]
        public async Task<WorkspaceConfigurationModel> CreateWorkspaceConfiguration([FromBody] SaveWorkspaceConfigurationModel model)
        {
            return await _configurationManager.CreateWorkspaceConfiguration(model);
        }

        [HttpPut]
        public async Task<WorkspaceConfigurationModel> UpdateWorkspaceConfiguration([FromBody] SaveWorkspaceConfigurationModel model)
        {
            return await _configurationManager.UpdateWorkspaceConfiguration(model);
        }

        [HttpDelete("{workspaceNumber}")]
        public async Task DeleteWorkspaceConfiguration(string workspaceNumber)
        {
            await _configurationManager.DeleteWorkspaceConfiguration(workspaceNumber);
        }
    }
}
