using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }
    [BsonElement("videoId")]
    public string VideoId { get; set; } = null!;
    [BsonElement("postedByUser")]
    public string UserId { get; set; } = null!;
    [BsonElement("text")]
    public string Text { get; set; } = null!;
    public long Date { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    [BsonElement("replies")]
    public List<string>? ReplyCommentIds { get; set; }
}