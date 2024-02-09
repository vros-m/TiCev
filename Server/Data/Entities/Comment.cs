using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Comment:MongoEntity
{

    public long Timestamp{ get; set; }
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Text { get; set; } = null!;
    public List<string> LikeIds { get; set; } = [];
}