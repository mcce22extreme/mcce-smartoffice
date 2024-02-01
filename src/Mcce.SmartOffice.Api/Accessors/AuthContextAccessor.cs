using Mcce.SmartOffice.Api.Extensions;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Accessors
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
