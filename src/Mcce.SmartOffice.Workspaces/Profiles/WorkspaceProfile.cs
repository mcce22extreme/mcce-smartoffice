using AutoMapper;
using Mcce.SmartOffice.Workspaces.Entities;
using Mcce.SmartOffice.Workspaces.Models;

namespace Mcce.SmartOffice.Workspaces.Profiles
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
