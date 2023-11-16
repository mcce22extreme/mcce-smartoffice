using Mcce.SmartOffice.Core.Extensions;
using Mcce.SmartOffice.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Mcce.SmartOffice.Core.Accessors
{
    public interface IAuthContextAccessor
    {
        UserInfoModel GetUserInfo();
    }

    internal class AuthContextAccessor : IAuthContextAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthContextAccessor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public UserInfoModel GetUserInfo()
        {
            return _contextAccessor.HttpContext.User.GetUserInfo();
        }
    }
}
