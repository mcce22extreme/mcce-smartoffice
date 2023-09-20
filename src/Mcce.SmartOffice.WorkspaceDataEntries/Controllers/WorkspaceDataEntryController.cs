using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.WorkspaceDataEntries.Managers;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkspaceDataEntryController : ControllerBase
    {
        private readonly IWorkspaceDataEntryManager _dataEntryManager;

        public WorkspaceDataEntryController(IWorkspaceDataEntryManager dataEntryManager)
        {
            _dataEntryManager = dataEntryManager;
        }

        [HttpGet("{workspaceNumber}")]
        public async Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(string workspaceNumber, DateTime? startDate, DateTime? endDate)
        {
            return await _dataEntryManager.GetWorkspaceDataEntries(workspaceNumber, startDate, endDate);
        }

        [HttpPost("{workspaceNumber}")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(string workspaceNumber, [FromBody] SaveWorkspaceDataEntryModel model)
        {
            return await _dataEntryManager.CreateWorkspaceDataEntry(workspaceNumber, model);
        }

        [HttpDelete("{workspaceNumber}")]
        [Authorize(AuthConstants.APP_ROLE_ADMINS)]
        public async Task DeleteWorkspaceDataEntry(string workspaceNumber)
        {
            await _dataEntryManager.DeleteWorkspaceDataEntries(workspaceNumber);
        }
    }
}
