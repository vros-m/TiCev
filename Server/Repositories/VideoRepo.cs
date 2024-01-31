using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Entities;
namespace TiCev.Server.Repositories;
public class VideoRepo(IMongoDatabase db, string collectionName = "videos")
{
    private readonly IMongoCollection<Video> _videos = db.GetCollection<Video>(collectionName);

    public async Task<List<Video>> GetAllVideosAsync()
    {
        return await _videos.Find(video => true).ToListAsync();
    }

    public async Task<Video?> GetVideoByIdAsync(string id)
    {
        var objId = new ObjectId(id);
        return await _videos.Find(video => video._id == objId).FirstOrDefaultAsync();
    }
}