using AutoMapper;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Profiles
{
    public class WorkspaceProfile : Profile
    {
        public WorkspaceProfile()
        {
            CreateMap<Workspace, WorkspaceModel>();

            CreateMap<SaveWorkspaceModel, Workspace>();
        }
    }
}
