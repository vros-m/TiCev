using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class Tag
{

    //tag mozda bude skroz zamenjen indeksiranjem
    //po listi tagova u video entitetu
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }
    [BsonElement("tagValue")]
    public string TagName { get; set; } = null!;

    //ovo bi bilo za search
    [BsonElement("videosWithTag")]
    public List<string>? AssociatedVideoIds { get; set; }
}