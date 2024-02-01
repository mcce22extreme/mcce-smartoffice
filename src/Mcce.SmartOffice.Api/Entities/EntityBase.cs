using System.ComponentModel.DataAnnotations;

namespace Mcce.SmartOffice.Api.Entities
{
    public interface IEntity
    {
        int Id { get; }
    }

    public abstract class EntityBase : IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
