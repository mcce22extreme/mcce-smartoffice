using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Enums;
using Mcce.SmartOffice.Api.Exceptions;
using Mcce.SmartOffice.Api.Generators;
using Mcce.SmartOffice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Api.Managers
{
    public interface IWorkspaceDataEntryManager
    {
        Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(string workspaceNumber, DateTime? startDate, DateTime? endDate);

        Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(string workspaceNumber, SaveWorkspaceDataEntryModel model);

        Task DeleteWorkspaceDataEntries(string workspaceNumber);
    }

    public class WorkspaceDataEntryManager : IWorkspaceDataEntryManager
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IWeiGenerator _weiGenerator;

        public WorkspaceDataEntryManager(AppDbContext dbContext, IMapper mapper, IWeiGenerator weiGenerator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _weiGenerator = weiGenerator;
        }

        public async Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(string workspaceNumber, DateTime? startDate, DateTime? endDate)
        {
            var entryQuery = _dbContext.WorkspaceDataEntries
                .Where(x => x.Workspace.WorkspaceNumber ==  workspaceNumber);

            if (startDate.HasValue)
            {
                entryQuery = entryQuery.Where(x => x.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                entryQuery = entryQuery.Where(x => x.Timestamp <= endDate.Value);
            }

            return await entryQuery
                .OrderByDescending(x => x.Timestamp)
                .ProjectTo<WorkspaceDataEntryModel>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(string workspaceNumber, SaveWorkspaceDataEntryModel model)
        {
            var entry = _mapper.Map<WorkspaceDataEntry>(model);

            var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if(workspace == null)
            {
                throw new NotFoundException($"The workspace '{workspaceNumber}' does not exist!");
            }

            var wei = _weiGenerator.GenerateWei(entry.Temperature, entry.Humidity, entry.Co2Level);

            entry.WorkspaceId = workspace.Id;
            entry.Wei = wei;
            workspace.Wei = wei;

            if (!model.Timestamp.HasValue)
            {
                entry.Timestamp = DateTime.UtcNow;
            }

            await _dbContext.WorkspaceDataEntries.AddAsync(entry);

            await _dbContext.SaveChangesAsync(auditInfoUpdateMode: AuditInfoUpdateMode.Suppress);

            return _mapper.Map<WorkspaceDataEntryModel>(entry);
        }

        public async Task DeleteWorkspaceDataEntries(string workspaceNumber)
        {
            await _dbContext.WorkspaceDataEntries
                .Where(x => x.Workspace.WorkspaceNumber == workspaceNumber)
                .ExecuteDeleteAsync();
        }
    }
}
