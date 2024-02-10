
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;

public class UserRepo(IMongoClient client):ARepo<User>(client,"users")
{
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await GetFirstByFieldAsync(user => user.Username == username);
    }
    public async Task<UserView?> GetViewByIdAsync(string id,string currentUserId)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument("_id", ObjectId.Parse(id) )),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "videos" },      // The name of the videos collection
                    { "localField", "VideoIds" },  // The field from the input documents (user.VideoIds)
                    { "foreignField", "_id" },          // The field from the documents of the "from" collection (videos)
                    { "as", "Videos" }                  // The name of the new array field to add
                }
            ),
            new BsonDocument("$project",new BsonDocument{
                {"PasswordHash",0},{"VideoIds",0},{"ProfilePicture",0}
            })
        };
        var res = (await _collection.AggregateAsync<UserViewDTO>(pipeline)).First();
        return DTOManager.FromDTOToUserView(res,currentUserId);
    }

    public async Task<List<Video>> GetPlaylistContent(string id)
    {

         var pipeline = new BsonDocument[]{
            new BsonDocument("$match", new BsonDocument("Playlists._id", new BsonObjectId(new ObjectId(id)))),
            new BsonDocument("$unwind", "$Playlists"),
            new BsonDocument("$match", new BsonDocument("Playlists._id", new BsonObjectId(new ObjectId(id)))),
            new BsonDocument("$unwind", "$Playlists.VideoIds"),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "videos" },
                { "localField", "Playlists.VideoIds" },
                { "foreignField", "_id" },
                { "as", "Videos" }
            }),  new BsonDocument("$unwind", "$Playlists.VideoIds"),
            new BsonDocument("$project",new BsonDocument{
            {"Videos",1},{"_id",0}
            }),
            new BsonDocument("$unwind","$Videos"),
            new BsonDocument("$replaceRoot",new BsonDocument("newRoot","$Videos")),
        new BsonDocument("$sort", new BsonDocument("_id", -1)),
        };

        return (await _collection.AggregateAsync<Video>(pipeline)).ToList();
    }
    public async Task<List<Video>> GetSubscriptionVideos(string id,int skip)
    {
        const int limit = 20;
        BsonDocument[] pipeline = [
            new("$match",new BsonDocument("_id",ObjectId.Parse(id))),new("$lookup",new BsonDocument
            {
                { "from", "users" },
                { "localField", "Subscriptions.ChannelId" },
                { "foreignField", "_id" },
                { "as", "Channels" }
            }),
            new("$lookup",new BsonDocument{
                { "from", "videos" },
                { "localField", "Channels.VideoIds" },
                { "foreignField", "_id" },
                { "as", "Feed" }
            }),new("$unwind","$Feed"),new("$replaceRoot",new BsonDocument("newRoot","$Feed")),
            new("$sort",new BsonDocument("_id",-1)),new("$skip",limit*skip),new("$limit",limit)
            
        ];

        var result = (await _collection.AggregateAsync<Video>(pipeline)).ToList();
        return result;
    }
}