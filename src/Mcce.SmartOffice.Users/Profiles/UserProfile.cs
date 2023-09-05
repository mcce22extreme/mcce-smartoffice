using AutoMapper;
using Mcce.SmartOffice.Users.Entities;
using Mcce.SmartOffice.Users.Models;

namespace Mcce22.SmartOffice.Users.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
