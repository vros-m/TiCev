using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;

public class Playlist : NestedMongoEntity
{
    public string Title { get; set; } = null!;
    [BsonIgnore]
    public string ChannelId { get; set; } = null!;
    [BsonIgnore]
    public string ChannelName { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> VideoIds { get; set; } = [];

}

public record class PlaylistViewDTO(ObjectId Id,string Title,ObjectId ChannelId,string ChannelName,List<Video> Videos);
public record class PlaylistView(string Id,string Title,string ChannelId,string ChannelName,List<VideoCardView> Videos);