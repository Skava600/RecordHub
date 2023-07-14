namespace RecordHub.Shared.Exceptions
{
    public class EntityNotFoundException : ArgumentException
    {
        private const string EntityNotFoundMessage = "Entity not found";

        public EntityNotFoundException(string paramname)
            : base(EntityNotFoundMessage, paramname)
        { }
    }
}
