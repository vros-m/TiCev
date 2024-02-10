using MongoDB.Bson.Serialization.Attributes;

namespace TiCev.Server.Data.Entities;

public class Subscription
{
    public string SubscriberId { get; set; } = null!;
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string ChannelId { get; set; } = null!;
    public string ChannelName { get; set; } = "";
}