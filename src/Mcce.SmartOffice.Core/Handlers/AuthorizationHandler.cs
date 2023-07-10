using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Serilog;

namespace Mcce.SmartOffice.Core.Handlers
{
    public class RbacRequirement : IAuthorizationRequirement
    {
        public string Role { get; }

        public RbacRequirement(string role)
        {
            Role = role;
        }
    }

    public class RealmAccessClaim
    {
        [JsonProperty("roles")]
        public string[] Roles { get; set; }
    }

    public class AuthorizationHandler : AuthorizationHandler<RbacRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RbacRequirement requirement)
        {
            try
            {
                // evaluate resource requirements
                if (context.User.IsInRole(requirement.Role))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }

                
                //if (userInfo.Roles?.Any(x => x == requirement.Role) == true)
                //{
                //    context.Succeed(requirement);                    
                //}
                //else
                //{
                //    context.Fail();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }    
}
