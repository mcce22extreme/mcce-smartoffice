using Mcce.SmartOffice.WorkspaceDataEntries.Managers;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
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

        [HttpGet]
        public async Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries([FromQuery] WorkspaceDataEntryQuery query)
        {
            return await _dataEntryManager.GetWorkspaceDataEntries(query);
        }

        [HttpPost]
        public async Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry([FromBody] SaveWorkspaceDataEntryModel model)
        {
            return await _dataEntryManager.CreateWorkspaceDataEntry(model);
        }

        [HttpDelete("{entryId}")]
        public async Task DeleteWorkspaceDataEntry(int entryId)
        {
            await _dataEntryManager.DeleteWorkspaceDataEntry(entryId);
        }

        [HttpDelete("deleteall")]
        public async Task DeleteAllEntries()
        {
            await _dataEntryManager.DeleteAllEntries();
        }
    }
}
