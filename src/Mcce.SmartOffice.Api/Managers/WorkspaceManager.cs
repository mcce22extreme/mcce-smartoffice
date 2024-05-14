using AutoMapper;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Enums;
using Mcce.SmartOffice.Api.Exceptions;
using Mcce.SmartOffice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Api.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetWorkspaces();

        Task<WorkspaceModel> GetWorkspace(string workspaceNumber);

        Task<WorkspaceModel> CreateWorkspace(SaveWorkspaceModel model);

        Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, SaveWorkspaceModel model);

        Task<WorkspaceModel> UpdateWorkspaceWei(string workspaceNumber, int wei);

        Task<bool> WorkspaceExists(string workspaceNumber);

        Task DeleteWorkspace(string workspaceNumber);
    }

    public class WorkspaceManager : IWorkspaceManager
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public WorkspaceManager(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<WorkspaceModel[]> GetWorkspaces()
        {
            var workspaces = await _dbContext.Workspaces
                .OrderBy(x => x.WorkspaceNumber)
                .ToListAsync();

            return workspaces
                .Select(_mapper.Map<WorkspaceModel>)
                .ToArray();
        }

        public async Task<WorkspaceModel> GetWorkspace(string workspaceNumber)
        {
            var workspace = await _dbContext.Workspaces
                .FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            return workspace == null
                ? throw new NotFoundException($"Could not find workspace '{workspaceNumber}'!")
                : _mapper.Map<WorkspaceModel>(workspace);
        }

        public async Task<WorkspaceModel> CreateWorkspace(SaveWorkspaceModel model)
        {
            var workspace = _mapper.Map<Workspace>(model);

            await _dbContext.Workspaces.AddAsync(workspace);

            await _dbContext.SaveChangesAsync();

            return await GetWorkspace(workspace.WorkspaceNumber);
        }

        public async Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, SaveWorkspaceModel model)
        {
            var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (workspace == null)
            {
                throw new NotFoundException($"Could not find workspace '{workspaceNumber}'!");
            }

            _mapper.Map(model, workspace);

            await _dbContext.SaveChangesAsync();

            return await GetWorkspace(workspaceNumber);
        }

        public async Task DeleteWorkspace(string workspaceNumber)
        {
            var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (workspace != null)
            {
                _dbContext.Workspaces.Remove(workspace);

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<WorkspaceModel> UpdateWorkspaceWei(string workspaceNumber, int wei)
        {
            var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (workspace == null)
            {
                throw new NotFoundException($"Could not find workspace '{workspaceNumber}'!");
            }

            workspace.Wei = wei;

            await _dbContext.SaveChangesAsync(auditInfoUpdateMode: AuditInfoUpdateMode.Suppress);

            return await GetWorkspace(workspaceNumber);
        }

        public async Task<bool> WorkspaceExists(string workspaceNumber)
        {
            return await _dbContext.Workspaces.AnyAsync(x => x.WorkspaceNumber == workspaceNumber);
        }
    }
}
