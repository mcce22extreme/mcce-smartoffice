using FluentValidation;

namespace Mcce.SmartOffice.Core.Providers
{
    public interface IValidationProvider
    {
        IEnumerable<IValidator> GetValidators();
    }

    public class ValidationProvider : IValidationProvider
    {
        private readonly IEnumerable<IValidator> _validators;

        public ValidationProvider(IEnumerable<IValidator> validators)
        {
            _validators = validators ?? Array.Empty<IValidator>();
        }

        public IEnumerable<IValidator> GetValidators()
        {
            return _validators;
        }
    }
}
