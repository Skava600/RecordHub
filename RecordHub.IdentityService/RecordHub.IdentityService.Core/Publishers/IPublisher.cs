namespace RecordHub.IdentityService.Core.Publishers
{
    public interface IPublisher<in T> where T : class
    {
        public Task PublishMessage(T message);
    }
}
