namespace Mcce.SmartOffice.Client.Models
{
    public interface IModel
    {
        int Id { get; }
    }

    public abstract class ModelBase : IModel
    {
        public int Id { get; set; }
    }
}
