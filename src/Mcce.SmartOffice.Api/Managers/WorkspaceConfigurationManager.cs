using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Exceptions;
using Mcce.SmartOffice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Api.Managers
{
    public interface IWorkspaceConfigurationManager
    {
        Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations();

        Task<WorkspaceConfigurationModel> GetWorkspaceConfiguration(string workspaceNumber);

        Task<WorkspaceConfigurationModel> GetWorkspaceConfigurationByUserName(string workspaceNumber, string userName);

        Task<WorkspaceConfigurationModel> SaveWorkspaceConfiguration(string workspaceNumber, SaveWorkspaceConfigurationModel model);

        Task DeleteWorkspaceConfiguration(string workspaceNumber);
    }

    public class WorkspaceConfigurationManager : IWorkspaceConfigurationManager
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthContextAccessor _contextAccessor;

        public WorkspaceConfigurationManager(AppDbContext dbContext, IMapper mapper, IAuthContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<WorkspaceConfigurationModel[]> GetWorkspaceConfigurations()
        {
            var currentUser = _contextAccessor.GetUserInfo();

            return await _dbContext.WorkspaceConfigurations
                .Where(x => x.UserName == currentUser.UserName)
                .OrderBy(x => x.CreatedUtc)
                .ProjectTo<WorkspaceConfigurationModel>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<WorkspaceConfigurationModel> GetWorkspaceConfiguration(string workspaceNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.UserName == currentUser.UserName && x.Workspace.WorkspaceNumber == workspaceNumber);

            return configuration == null
                ? throw new NotFoundException($"Could not find configuration for user '{currentUser.UserName}' and workspace '{workspaceNumber}'!")
                : _mapper.Map<WorkspaceConfigurationModel>(configuration);
        }

        public async Task<WorkspaceConfigurationModel> GetWorkspaceConfigurationByUserName(string workspaceNumber, string userName)
        {
            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.UserName == userName && x.Workspace.WorkspaceNumber == workspaceNumber);

            return configuration == null ? null : _mapper.Map<WorkspaceConfigurationModel>(configuration);
        }

        public async Task<WorkspaceConfigurationModel> SaveWorkspaceConfiguration(string workspaceNumber, SaveWorkspaceConfigurationModel model)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (workspace == null)
            {
                throw new NotFoundException($"The workspace '{workspaceNumber}' does not exist!");
            }

            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.Workspace.WorkspaceNumber == workspaceNumber && x.UserName == currentUser.UserName);
            
            if (configuration == null)
            {
                configuration = new WorkspaceConfiguration
                {
                    WorkspaceId = workspace.Id,
                    UserName = currentUser.UserName,
                };

                await _dbContext.WorkspaceConfigurations.AddAsync(configuration);
            }

            _mapper.Map(model, configuration);

            await _dbContext.SaveChangesAsync();

            return await GetWorkspaceConfiguration(workspaceNumber);
        }

        public async Task DeleteWorkspaceConfiguration(string workspaceNumber)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = await _dbContext.WorkspaceConfigurations.FirstOrDefaultAsync(x =>
                x.Workspace.WorkspaceNumber == workspaceNumber &&
                x.UserName == currentUser.UserName);

            if (configuration != null)
            {
                _dbContext.WorkspaceConfigurations.Remove(configuration);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
