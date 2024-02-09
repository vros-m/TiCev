using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Tag :MongoEntity
{
    public string TagName { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> AssociatedVideoIds { get; set; }=[];
}