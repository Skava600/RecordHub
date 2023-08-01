using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RecordHub.ChatService.Domain.Models;
using RecordHub.ChatService.Infrastructure.Config;

namespace RecordHub.ChatService.Infrastructure.Data.Repos
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly IMongoCollection<Room> _roomsCollection;

        public RoomsRepository(IOptions<ChatStoreDatabaseConfig> options)
        {
            var mongoClient = new MongoClient(
            options.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                options.Value.DatabaseName);

            _roomsCollection = mongoDatabase.GetCollection<Room>(
                options.Value.ChatsCollectionName);

            var indexOptions = new CreateIndexOptions { Unique = true };
            var roomBuilder = Builders<Room>.IndexKeys;
            var indexModel = new CreateIndexModel<Room>(roomBuilder.Ascending(x => x.RoomId));
            _roomsCollection.Indexes.CreateOne(indexModel);
        }

        public async Task CreateAsync(Room newBook)
        {
            await _roomsCollection.InsertOneAsync(newBook);
        }

        public async Task<List<Room>> GetAsync()
        {
            return await _roomsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Room?> GetAsync(string roomId)
        {
            return await _roomsCollection.Find(x => x.RoomId == roomId).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(string roomId)
        {
            await _roomsCollection.DeleteOneAsync(x => x.RoomId == roomId);
        }

        public async Task UpdateAsync(string roomId, Room updatedBook)
        {
            await _roomsCollection.ReplaceOneAsync(x => x.RoomId == roomId, updatedBook);
        }
    }
}
