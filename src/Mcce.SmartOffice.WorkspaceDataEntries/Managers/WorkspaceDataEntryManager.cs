using AutoMapper;
using Mcce.SmartOffice.Common.Constants;
using Mcce.SmartOffice.Common.Services;
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
        private readonly IMessageService _messageService;

        public WorkspaceDataEntryManager(AppDbContext dbContext, IMapper mapper, IWeiGenerator weiGenerator, IMessageService messageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _weiGenerator = weiGenerator;
            _messageService = messageService;
        }

        public async Task<WorkspaceDataEntryModel[]> GetWorkspaceDataEntries(string workspaceNumber, DateTime? startDate, DateTime? endDate)
        {
            var entryQuery = _dbContext.WorkspaceDataEntries
                .Where(x => x.WorkspaceNumber ==  workspaceNumber);                

            if (startDate.HasValue)
            {
                entryQuery = entryQuery.Where(x => x.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                entryQuery = entryQuery.Where(x => x.Timestamp <= endDate.Value);
            }

            var workspaceData = await entryQuery
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return workspaceData
                .Select(_mapper.Map<WorkspaceDataEntryModel>)
                .ToArray();
        }

        public async Task<WorkspaceDataEntryModel> CreateWorkspaceDataEntry(string workspaceNumber, SaveWorkspaceDataEntryModel model)
        {
            var entry = _mapper.Map<WorkspaceDataEntry>(model);

            entry.WorkspaceNumber = workspaceNumber;
            entry.Wei = _weiGenerator.GenerateWei(entry.Temperature, entry.Humidity, entry.Co2Level);

            if (!model.Timestamp.HasValue)
            {
                entry.Timestamp = DateTime.UtcNow;
            }

            await _dbContext.WorkspaceDataEntries.AddAsync(entry);

            await _dbContext.SaveChangesAsync();

            await _messageService.Publish(MessageTopics.TOPIC_WEI_UPDATED, new
            {
                entry.WorkspaceNumber,
                entry.Wei
            });

            return _mapper.Map<WorkspaceDataEntryModel>(entry);
        }

        public async Task DeleteWorkspaceDataEntries(string workspaceNumber)
        {
            await _dbContext.WorkspaceDataEntries
                .Where(x => x.WorkspaceNumber == workspaceNumber)
                .ExecuteDeleteAsync();

            //var entries = await _dbContext.WorkspaceDataEntries
            //    .Where(x => x.WorkspaceNumber == workspaceNumber)
            //    .ToListAsync();

            //foreach (var entry in entries)
            //{
            //    _dbContext.WorkspaceDataEntries.Remove(entry);
            //}

            //await _dbContext.SaveChangesAsync();
        }
    }
}
