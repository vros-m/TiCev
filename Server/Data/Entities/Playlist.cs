using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;

public class Playlist
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }
    [BsonElement("title")]
    public string Title { get; set; } = null!;
    [BsonElement("userId")]
    public string UserId { get; set; } = null!;
    [BsonElement("videoIds")]
    public List<string>? VideoIds { get; set; }
    public bool IsPublic { get; set; } = false;
    [BsonElement("playlistUrl")]
    public string PlaylistUrl { get; set; } = null!;
}