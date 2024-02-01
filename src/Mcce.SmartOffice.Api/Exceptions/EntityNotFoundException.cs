namespace Mcce.SmartOffice.Api.Exceptions
{
    public class EntityNotFoundException<T> : NotFoundException
    {
        public EntityNotFoundException(int entityId)
            : base($"Could not find {typeof(T).Name} with id '{entityId}'!")
        {
        }

        public EntityNotFoundException(string entityName)
            : base($"Could not find {typeof(T).Name} with name '{entityName}'!")
        {
        }
    }
}
