using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Comment:NestedMongoEntity
{

    [BsonIgnore]
    public long Timestamp{ get{
            return (long)Id.Timestamp * 1000L;
    }}
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = "";
    public string ProfilePicture { get; set; } = "";
    public string Text { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Likes { get; set; } = [];
    public int ReplyingToId{ get; set; }

    public string ReplyingToUserId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string VideoId { get; set; } = null!;

    [BsonIgnore]
    public string VideoPosterId { get; set; } = null!;
}

public record class CommentView(long Timestamp,string Id,int ReplyToId,string VideoId,int Likes,bool IsLiked,
string Text,string UserId,string Username,string ProfilePicture);
