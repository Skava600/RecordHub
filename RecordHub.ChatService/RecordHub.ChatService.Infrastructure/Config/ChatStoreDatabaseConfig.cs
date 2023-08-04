namespace RecordHub.ChatService.Infrastructure.Config
{
    public class ChatStoreDatabaseConfig
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ChatsCollectionName { get; set; } = null!;
    }
}
