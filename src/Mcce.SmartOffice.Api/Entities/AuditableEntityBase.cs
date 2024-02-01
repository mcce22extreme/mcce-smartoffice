namespace Mcce.SmartOffice.Api.Entities
{
    public interface IAuditableEntity : IEntity
    {
        DateTime? CreatedUtc { get; }

        string Creator { get; }

        DateTime? ModifiedUtc { get; }

        string Modifier { get; }
    }

    public abstract class AuditableEntityBase : EntityBase, IAuditableEntity
    {
        public DateTime? CreatedUtc { get; set; }

        public string Creator { get; set; }

        public DateTime? ModifiedUtc { get; set; }

        public string Modifier { get; set; }
    }
}
