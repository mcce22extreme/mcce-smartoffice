using AutoMapper;
using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.WorkspaceDataEntries.Generators;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Managers
{
    public interface IWorkspaceDataEntryManager
    {
        Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(WorkspaceDataEntryQuery query);

        Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(SaveWorkspaceDataEntryModel model);

        Task DeleteWorkspaceDataEntry(int entryId);

        Task DeleteAllEntries();
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

        public async Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(WorkspaceDataEntryQuery query)
        {
            var entryQuery = _dbContext.Entries
                .OrderByDescending(x => x.Timestamp)
                .AsQueryable();

            if (query.WorkspaceNumber.HasValue())
            {
                entryQuery = entryQuery.Where(x => x.WorkspaceNumber == query.WorkspaceNumber);
            }

            var workspaceData = await entryQuery.ToListAsync();

            return workspaceData
                .Select(_mapper.Map<Models.WorkspaceDataEntryModel>)
                .ToArray();
        }

        public async Task<Models.WorkspaceDataEntryModel> CreateWorkspaceDataEntry(SaveWorkspaceDataEntryModel model)
        {
            var entry = _mapper.Map<Entities.WorkspaceDataEntry>(model);

            entry.WorkspaceNumber = model.WorkspaceNumber;
            entry.Wei = _weiGenerator.GenerateWei(entry.Temperature, entry.Humidity, entry.Co2Level);

            if (!model.Timestamp.HasValue)
            {
                entry.Timestamp = DateTime.UtcNow;
            }

            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Entries.AddAsync(entry);

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return _mapper.Map<Models.WorkspaceDataEntryModel>(entry);
        }

        public async Task DeleteWorkspaceDataEntry(int entryId)
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Entries
                .Where(x => x.Id == entryId)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
        }

        public async Task DeleteAllEntries()
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Entries
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
        }
    }
}
