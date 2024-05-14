using System.Net;
using FluentValidation;
using Mcce.SmartOffice.Api.Exceptions;
using Mcce.SmartOffice.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace Mcce.SmartOffice.Api.Handlers
{
    public static class GlobalExceptionHandler
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error(contextFeature.Error.Message, contextFeature.Error);

                        var errorModel = CreateErrorModel(contextFeature.Error);

                        context.Response.StatusCode = (int)errorModel.StatusCode;
                        await context.Response.WriteAsync(errorModel.ToString());
                    }
                });
            });
        }

        private static ErrorModel CreateErrorModel(Exception exception)
        {
            return exception switch
            {
                NotFoundException nex => new ErrorModel
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = nex.Message,
                },
                ValidationException vex => new ValidationErrorModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = vex.Message ?? "One or more validation errors occured!",
                    Errors = vex.Errors.Select(e => new ValidationError
                    {
                        ErrorCode = e.ErrorCode,
                        ErrorMessage = e.ErrorMessage,
                        PropertyName = e.PropertyName,
                    }).ToArray(),
                },
                _ => new ErrorModel
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = exception.Message + ", " + exception.StackTrace,
                },
            };
        }
    }
}
