using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RecordHub.ChatService.Domain.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string RoomId { get; set; }

        public List<Message> Messages { get; set; }
    }
}
