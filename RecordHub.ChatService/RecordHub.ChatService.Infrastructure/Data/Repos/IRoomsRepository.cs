using RecordHub.ChatService.Domain.Models;

namespace RecordHub.ChatService.Infrastructure.Data.Repos
{
    public interface IRoomsRepository
    {
        public Task<List<Room>> GetAsync();

        public Task<Room?> GetAsync(string roomId);

        public Task CreateAsync(Room newBook);

        public Task UpdateAsync(string id, Room updatedBook);

        public Task RemoveAsync(string id);
    }
}
