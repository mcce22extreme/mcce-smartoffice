﻿using Mcce.SmartOffice.Workspaces.Managers;
using Mcce.SmartOffice.Workspaces.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mcce.SmartOffice.Workspaces.Controllers
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
        public async Task<WorkspaceModel> CreateWorkspace([FromBody] SaveWorkspaceModel model)
        {
            return await _workspaceManager.CreateWorkspace(model);
        }

        [HttpPut("{workspaceNumber}")]
        public async Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, [FromBody] SaveWorkspaceModel model)
        {
            return await _workspaceManager.UpdateWorkspace(workspaceNumber, model);
        }

        [HttpDelete("{workspaceNumber}")]
        public async Task DeleteWorkspace(string workspaceNumber)
        {
            await _workspaceManager.DeleteWorkspace(workspaceNumber);
        }
    }
}