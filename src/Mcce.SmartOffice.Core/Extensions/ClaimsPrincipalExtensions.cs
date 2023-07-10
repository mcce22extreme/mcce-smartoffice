using System.Security.Claims;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Models;

namespace Mcce.SmartOffice.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static UserInfoModel GetUserInfo(this ClaimsPrincipal principal)
        {
            var user = new UserInfoModel();

            if (principal.Identity.IsAuthenticated)
            {
                user.UserName = principal.FindFirstValue("preferred_username");
                user.FirstName = principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
                user.LastName = principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
                user.Email = principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                user.IsAdmin = principal.IsInRole(AuthConstants.APP_ROLE_ADMINS);
            }
            else
            {
                user.UserName = "fallback";
                user.FirstName = "Fallback";
                user.LastName = "User";
                user.IsAdmin = true;
            }

            return user;
        }
    }
}
