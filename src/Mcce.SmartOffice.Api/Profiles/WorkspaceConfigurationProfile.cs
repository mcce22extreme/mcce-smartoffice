using AutoMapper;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Profiles
{
    public class WorkspaceConfigurationProfile : Profile
    {
        public WorkspaceConfigurationProfile()
        {
            CreateMap<WorkspaceConfiguration, WorkspaceConfigurationModel>()
                .ForMember(d => d.WorkspaceNumber, opt => opt.MapFrom(s => s.Workspace.WorkspaceNumber));

            CreateMap<SaveWorkspaceConfigurationModel, WorkspaceConfiguration>();
        }
    }
}
