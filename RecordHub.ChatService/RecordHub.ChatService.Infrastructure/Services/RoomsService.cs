using RecordHub.ChatService.Application.Services;
using RecordHub.ChatService.Domain.Models;
using RecordHub.ChatService.Infrastructure.Data.Repos;
using RecordHub.Shared.Exceptions;

namespace RecordHub.ChatService.Infrastructure.Services
{
    public class RoomsService : IRoomsService
    {
        private IRoomsRepository _roomsRepository;

        public RoomsService(IRoomsRepository roomsRepository)
        {
            _roomsRepository = roomsRepository;
        }

        public async Task<Room> CreateAsync(Room newRoom)
        {
            await _roomsRepository.CreateAsync(newRoom);

            return newRoom;
        }

        public Task<List<Room>> GetAsync() => _roomsRepository.GetAsync();

        public async Task<Room?> GetAsync(string roomId)
        {
            var book = await _roomsRepository.GetAsync(roomId);

            return book;
        }

        public async Task DeleteAsync(string id)
        {
            var book = await _roomsRepository.GetAsync(id);

            if (book is null)
            {
                throw new EntityNotFoundException(nameof(id));
            }

            await _roomsRepository.RemoveAsync(id);
        }

        public async Task<Room> AddMessagesAsync(string roomId, IEnumerable<Message> messages)
        {
            var room = await _roomsRepository.GetAsync(roomId);

            if (room is null)
            {
                throw new EntityNotFoundException(nameof(roomId));
            }

            room.Messages.AddRange(messages);
            await _roomsRepository.UpdateAsync(roomId, room);

            return room;
        }
    }
}
