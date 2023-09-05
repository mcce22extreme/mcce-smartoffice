using AutoMapper;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.WorkspaceConfigurations.Entities;
using Mcce.SmartOffice.WorkspaceConfigurations.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceConfigurations.Managers
{
    public interface IWorkspaceConfigurationManager
    {
        Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations();

        Task<WorkspaceConfigurationModel> GetWorkspaceConfiguration(string workspaceNumber);

        Task<WorkspaceConfigurationModel> CreateWorkspaceConfiguration(SaveWorkspaceConfigurationModel model);

        Task<WorkspaceConfigurationModel> UpdateWorkspaceConfiguration(SaveWorkspaceConfigurationModel model);

        Task DeleteWorkspaceConfiguration(string workspaceNumber);
    }

    public class WorkspaceConfigurationManager : IWorkspaceConfigurationManager
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public WorkspaceConfigurationManager(AppDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations()
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configurations = await _dbContext.WorkspaceConfigurations
                .Where(x => x.UserName == currentUser.UserName)
                .OrderBy(x => x.CreatedUtc)
                .ToListAsync();

            return configurations
                .Select(_mapper.Map<WorkspaceConfigurationModel>)
                .ToArray();
        }

        public async Task<WorkspaceConfigurationModel> GetWorkspaceConfiguration(string workspaceNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.UserName == currentUser.UserName && x.WorkspaceNumber == workspaceNumber);

            return configuration == null
                ? throw new NotFoundException($"Could not find configuration for user '{currentUser.UserName}' and workspace '{workspaceNumber}'!")
                : _mapper.Map<WorkspaceConfigurationModel>(configuration);
        }

        public async Task<WorkspaceConfigurationModel> CreateWorkspaceConfiguration(SaveWorkspaceConfigurationModel model)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = _mapper.Map(model, new WorkspaceConfiguration
            {
                UserName = currentUser.UserName,
            });

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.WorkspaceConfigurations.AddAsync(configuration);

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetWorkspaceConfiguration(model.WorkspaceNumber);
        }

        public async Task<WorkspaceConfigurationModel> UpdateWorkspaceConfiguration(SaveWorkspaceConfigurationModel model)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.UserName == currentUser.UserName && x.WorkspaceNumber == model.WorkspaceNumber);

            if (configuration == null)
            {
                throw new NotFoundException($"Could not find configuration for user '{currentUser.UserName}' and workspace '{model.WorkspaceNumber}'!");
            }

            _mapper.Map(model, configuration);

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetWorkspaceConfiguration(model.WorkspaceNumber);
        }

        public async Task DeleteWorkspaceConfiguration(string workspaceNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.WorkspaceConfigurations
                .Where(x => x.UserName == currentUser.UserName && x.WorkspaceNumber == workspaceNumber)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
        }
    }
}
