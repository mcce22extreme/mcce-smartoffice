using AutoMapper;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Mcce.SmartOffice.WorkspaceDataEntries.Generators;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Managers
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
                .Where(x => x.WorkspaceNumber ==  workspaceNumber)
                .OrderByDescending(x => x.Timestamp)
                .AsQueryable();

            var workspaceData = await entryQuery.ToListAsync();

            return workspaceData
                .Select(_mapper.Map<WorkspaceDataEntryModel>)
                .ToArray();
        }

        public async Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(string workspaceNumber, SaveWorkspaceDataEntryModel model)
        {
            var entry = _mapper.Map<WorkspaceDataEntry>(model);

            entry.EntryId = Guid.NewGuid().ToString();
            entry.WorkspaceNumber = workspaceNumber;
            entry.Wei = _weiGenerator.GenerateWei(entry.Temperature, entry.Humidity, entry.Co2Level);

            if (!model.Timestamp.HasValue)
            {
                entry.Timestamp = DateTime.UtcNow;
            }

            await _dbContext.WorkspaceDataEntries.AddAsync(entry);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<WorkspaceDataEntryModel>(entry);
        }

        public async Task DeleteWorkspaceDataEntries(string workspaceNumber)
        {
            var entry = await _dbContext.WorkspaceDataEntries.FirstOrDefaultAsync(x => x.WorkspaceNumber == workspaceNumber);

            if (entry != null)
            {
                _dbContext.WorkspaceDataEntries.Remove(entry);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
