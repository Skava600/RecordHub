using RecordHub.ChatService.Domain.Models;

namespace RecordHub.ChatService.Application.Services
{
    public interface IRoomsService
    {
        public Task<List<Room>> GetAsync();

        public Task<Room?> GetAsync(string id);

        public Task<Room> CreateAsync(Room newBook);

        public Task<Room> AddMessagesAsync(string id, IEnumerable<Message> messages);

        public Task DeleteAsync(string id);
    }
}
