using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.GridFS;
using TiCev.Server.Business.Repos;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Services;

public class VideoService(VideoRepo repo):AService<Video>(repo)
{
    private new readonly VideoRepo _repo = repo;
    public async Task<Video> UploadVideoAsync(VideoDTO dto)
    {
        return await _repo.UploadVideoAsync(dto);
    }

    public async Task DeleteVideoAsync(string id)
    {
        await _repo.DeleteVideoAsync(id);
    }

    /// <summary>
/// [deprecated]
/// 
/// Use UploadVideoAsync
/// </summary>
/// <param name="document"></param>
/// <returns></returns>
    public override Task<Video> AddAsync(Video document)
    {
        return base.AddAsync(document);
    }
    public async Task UpdateDescription(string videoId,string newDesc)
    {
        var update = Builders<Video>.Update.Set(v => v.Description, newDesc);
        await _repo.UpdateAsync(v => v.Id == ObjectId.Parse(videoId), update);
    }

    public async Task RateVideo(string videoId,string userId,double rating=0.0)
    {
        if(rating !=0.0)
        {
            await _repo.InsertIntoListAsync(v=>v.Id==ObjectId.Parse(videoId),v=>v.Ratings,Tuple.Create(userId,rating));
        }
        else
        {
            await _repo.RemoveFromListAsync(v => v.Id == ObjectId.Parse(videoId), v => v.Ratings, item => item.Item1 == userId);
        }
    }

    public async Task<GridFSDownloadStream> GetVideoAsync(string id)
    {
        return await _repo.GetVideoAsync(id);
    }

    public async Task<Tuple<byte[],string>> GetThumbnailAsync(string id)
    {
        return await _repo.GetThumbnailAsync(id);
    }

    public async Task IncrementViews(string id)
    {
        await _repo.IncrementViews(id);
    }
}