﻿using AutoMapper;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Workspaces.Entities;
using Mcce.SmartOffice.Workspaces.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Workspaces.Managers
{
    public interface IWorkspaceManager
    {
        Task<WorkspaceModel[]> GetWorkspaces();

        Task<WorkspaceModel> GetWorkspace(string workspaceNumber);

        Task<WorkspaceModel> CreateWorkspace(SaveWorkspaceModel model);

        Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, SaveWorkspaceModel model);

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

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Workspaces.AddAsync(workspace);

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetWorkspace(workspace.WorkspaceNumber);
        }

        public async Task<WorkspaceModel> UpdateWorkspace(string workspaceNumber, SaveWorkspaceModel model)
        {
            var workspace = await _dbContext.Workspaces
                .FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (workspace == null)
            {
                throw new NotFoundException($"Could not find workspace '{workspaceNumber}'!");
            }

            _mapper.Map(model, workspace);

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetWorkspace(workspaceNumber);
        }

        public async Task DeleteWorkspace(string workspaceNumber)
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Workspaces
                .Where(x => x.WorkspaceNumber == workspaceNumber)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
        }
    }
}
