using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Serilog.Events;

namespace Mcce.SmartOffice.Api.Attributes
{
    public class OperationLoggerAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                var operationId = Guid.NewGuid().GetHashCode().ToString("x8");
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

                Log.Debug($"[In ] {operationId} {actionDescriptor?.ActionName} ({context.HttpContext.Request.Path})");

                var sw = Stopwatch.StartNew();

                await next();

                Log.Debug($"[Out] {operationId} {actionDescriptor?.ActionName} ({sw.Elapsed})");
            }
            else
            {
                await next();
            }
        }
    }
}
