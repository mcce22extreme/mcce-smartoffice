using AutoMapper;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Mcce.SmartOffice.WorkspaceDataEntries.Models;

namespace Mcce.SmartOffice.WorkspaceDataEntries.Profiles
{
    public class WorkspaceDataEntryProfile : Profile
    {
        public WorkspaceDataEntryProfile()
        {
            CreateMap<WorkspaceDataEntry, WorkspaceDataEntryModel>();

            CreateMap<SaveWorkspaceDataEntryModel, WorkspaceDataEntry>();
        }
    }
}
