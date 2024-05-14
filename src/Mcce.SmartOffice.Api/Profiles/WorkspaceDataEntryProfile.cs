using AutoMapper;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Profiles
{
    public class WorkspaceDataEntryProfile : Profile
    {
        public WorkspaceDataEntryProfile()
        {
            CreateMap<WorkspaceDataEntry, WorkspaceDataEntryModel>()
                .ForMember(d => d.WorkspaceNumber, opt => opt.MapFrom(s => s.Workspace.WorkspaceNumber));

            CreateMap<SaveWorkspaceDataEntryModel, WorkspaceDataEntry>();
        }
    }
}
