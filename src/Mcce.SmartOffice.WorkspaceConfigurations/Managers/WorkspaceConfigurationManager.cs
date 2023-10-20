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

        Task<WorkspaceConfigurationModel> GetWorkspaceConfigurationByUserName(string workspaceNumber, string userName);

        Task<WorkspaceConfigurationModel> SaveWorkspaceConfiguration(string workspaceNumber, SaveWorkspaceConfigurationModel model);

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

            return await GetWorkspaceConfigurationByUserName(workspaceNumber, currentUser.UserName);
        }


        public async Task<WorkspaceConfigurationModel> GetWorkspaceConfigurationByUserName(string workspaceNumber, string userName)
        {
            var configuration = await _dbContext.WorkspaceConfigurations
                .FirstOrDefaultAsync(x => x.UserName == userName && x.WorkspaceNumber == workspaceNumber);

            return configuration == null
                ? throw new NotFoundException($"Could not find configuration for user '{userName}' and workspace '{workspaceNumber}'!")
                : _mapper.Map<WorkspaceConfigurationModel>(configuration);
        }

        public async Task<WorkspaceConfigurationModel> SaveWorkspaceConfiguration(string workspaceNumber, SaveWorkspaceConfigurationModel model)
        {
            var currentUser = _contextAccessor.GetUserInfo();

            var configuration = await _dbContext.WorkspaceConfigurations.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if(configuration == null)
            {
                configuration = new WorkspaceConfiguration
                {
                    ConfigurationNumber = $"{workspaceNumber}-{currentUser.UserName}",
                    WorkspaceNumber = workspaceNumber,
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

            var configuration = await _dbContext.WorkspaceConfigurations.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber &&  x.UserName == currentUser.UserName);

            if (configuration != null)
            {
                _dbContext.WorkspaceConfigurations.Remove(configuration);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
