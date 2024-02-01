using FluentValidation;
using Mcce.SmartOffice.Api.Providers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mcce.SmartOffice.Api.Attributes
{
    public class OperationValidatorAttribute : ActionFilterAttribute
    {
        private readonly IValidationProvider _validationProvider;

        public OperationValidatorAttribute(IValidationProvider validationProvider)
        {
            _validationProvider = validationProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var validators = _validationProvider?.GetValidators();
            var payload = GetPayload(context);

            if (validators.Any() && payload != null)
            {
                var validationContext = new ValidationContext<object>(payload);

                var errors = validators
                        ?.Where(x => x.CanValidateInstancesOfType(payload.GetType()))
                        ?.Select(x => x.Validate(validationContext))
                        ?.SelectMany(result => result.Errors)
                        ?.ToList();

                if (errors?.Count > 0)
                {
                    throw new ValidationException("One or more validation errors occured!", errors);
                }
            }
        }

        private object GetPayload(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("model", out var payload))
            {
                context.ActionArguments.TryGetValue("query", out payload);
            }

            return payload;
        }
    }
}
