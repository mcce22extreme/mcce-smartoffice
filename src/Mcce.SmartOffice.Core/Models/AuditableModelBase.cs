namespace Mcce.SmartOffice.Core.Models
{
    public class AuditableModelBase : ModelBase
    {
        public DateTime? CreatedUtc { get; set; }

        public string Creator { get; set; }

        public DateTime? ModifiedUtc { get; set; }

        public string Modifier { get; set; }
    }
}
