namespace TiCev.Server.Data.Entities;

using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Video
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }
    [BsonElement("userId")]
    public string UserId { get; set; } = null!;
    [BsonElement("title")]
    public string Title { get; set; } = null!;
    [BsonElement("description")]
    public string? Description { get; set; }

    // public List<string>? Tags { get; set; }

    public long UploadDate { get; set; }
    public string? Thumbnail { get; set; }
    [BsonElement("videoUrl")]
    public string VideoURL { get; set; } = null!;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}
