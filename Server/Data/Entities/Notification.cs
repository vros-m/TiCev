using MongoDB.Bson.Serialization.Attributes;

namespace TiCev.Server.Data.Entities;

public class Notification:NestedMongoEntity
{
    public string Message { get; set; } = null!;
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string SenderId { get; set; } = null!;

    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string VideoId { get; set; } = null!;

    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string RecipientId { get; set; } = null!; 

    [BsonIgnore]
    public int Timestamp{ get{
            return Id.Timestamp;
    }}
}
