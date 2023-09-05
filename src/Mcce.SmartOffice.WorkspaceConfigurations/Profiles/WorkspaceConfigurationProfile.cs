using AutoMapper;
using Mcce.SmartOffice.WorkspaceConfigurations.Entities;
using Mcce.SmartOffice.WorkspaceConfigurations.Models;

namespace Mcce.SmartOffice.WorkspaceConfigurations.Profiles
{
    public class WorkspaceConfigurationProfile : Profile
    {
        public WorkspaceConfigurationProfile()
        {
            CreateMap<WorkspaceConfiguration, WorkspaceConfigurationModel>();

            CreateMap<SaveWorkspaceConfigurationModel, WorkspaceConfiguration>();
        }
    }
}
