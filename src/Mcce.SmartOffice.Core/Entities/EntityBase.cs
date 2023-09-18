namespace Mcce.SmartOffice.Core.Entities
{
    public interface IEntity
    {
        int Id { get; }
    }

    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }
    }
}
