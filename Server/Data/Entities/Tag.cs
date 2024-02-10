using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Tag :MongoEntity
{
    public string TagName { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string VideoId { get; set; } = null!;

    public static List<Tag> ExtractTags(Video video)
    {
        return video.Tags.Select(tag => new Tag
        {
            TagName=tag,
            VideoId=video.ObjectId
        }).ToList();
    }
}