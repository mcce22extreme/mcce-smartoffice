using Mcce.SmartOffice.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Mcce.SmartOffice.Core.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        public static UserInfoModel GetUserInfo(this IHttpContextAccessor context)
        {
            return context.HttpContext.User.GetUserInfo();
        }
    }
}
