using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TiCev.Server.Data.Entities;
public class User :MongoEntity
{
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Bio { get; set; } = "";
    public string ProfilePicture { get; set; } = "";
    public DateTime Birthday { get; set; }
    public string Gender { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> VideoIds  { get; set; } = [];
    public List<Subscription> Subscriptions { get; set; } = [];
    public List<string> Subscribers { get; set; } = [];
    public List<Playlist> Playlists { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
}

public record class SimpleUser(string Id,string Username,List<Notification> Notifications,string UserPhoto="");
public record class UserViewDTO(string Username,string Email,string Bio,DateTime Birthday,string Gender,List<Video> Videos,
List<Subscription> Subscriptions,List<string> Subscribers, List<Playlist> Playlists,ObjectId Id);

public record class UserView(string Username,string Email,string Bio,DateTime Birthday,string Gender,List<VideoCardView> Videos,
List<Subscription> Subscriptions, List<Playlist> Playlists,string Id,bool IsSubscribed);

