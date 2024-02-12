using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TiCev.Server.Data.Entities;

public abstract class MongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    [BsonIgnore]
    [JsonPropertyName("id")]
    public string ObjectId{ get {
            return Id.ToString();
    } }
}