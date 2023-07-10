namespace Mcce.SmartOffice.Core.Models
{
    public class ValidationErrorModel : ErrorModel
    {
        public ValidationError[] Errors { get; set; }
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
