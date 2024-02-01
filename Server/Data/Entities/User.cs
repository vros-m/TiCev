using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TiCev.Server.Data.Entities;
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = null!;
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;
    [BsonElement("email")]
    public string Email { get; set; } = null!;

    // public string FirstName { get; set; } = String.Empty;
    // public string LastName { get; set; } = String.Empty;
    // public string? ProfilePicture { get; set; }

    [BsonElement("videosCreated")]
    public List<string>? CreatedVideoIds { get; set; }
    [BsonElement("videosLiked")]
    public List<string>? LikedVideoIds { get; set; }

    //mozda da ima mogucnost da subscribeuje na kanale    
    public List<string>? SubscriptionsIds { get; set; }

    public User()
    {
        CreatedVideoIds = new List<string>();
        LikedVideoIds = new List<string>();
        SubscriptionsIds = new List<string>();
    }
}