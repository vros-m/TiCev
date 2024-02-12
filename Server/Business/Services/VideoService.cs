using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.GridFS;
using TiCev.Server.Business.Repos;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;
using TiCev.Server.Error;

namespace TiCev.Server.Business.Services;

public class VideoService(VideoRepo repo,UserService userService,TagRepo tagRepo):AService<Video>(repo)
{
    private new readonly VideoRepo _repo = repo;
    private readonly UserService _userService = userService;

    private readonly TagRepo _tagRepo = tagRepo;
    public async Task<Video> UploadVideoAsync(VideoDTO dto)
    {
        var vid =await _repo.UploadVideoAsync(dto);
        await _userService.InsertIntoListAsync(u => u.Id == ObjectId.Parse(vid.ChannelId), u => u.VideoIds,
        vid.Id.ToString());
        var tags =Data.Entities.Tag.ExtractTags(vid);
        if(tags.Count!=0)
            await _tagRepo.AddManyAsync(tags);
        List<string> subs = (await _userService.GetByIdAsync(dto.ChannelId))!.Subscribers;
        List<Task> tasks = [];
        foreach(var userId in subs)
        {
            Notification notification = DTOManager.FromVideoToNotification(vid, userId);
            tasks.Add(_userService.AddNotification(notification));
        }
        Task.WaitAll([.. tasks]);

        return vid;
    }

    public async Task DeleteVideoAsync(string id)
    {
        var vid =await _repo.DeleteVideoAsync(id);
        await _userService.RemoveFromListAsync(u => u.Id == ObjectId.Parse(vid.ChannelId), u => u.VideoIds,id);
        await _userService.UpdateManyAsync(Builders<User>.Filter.ElemMatch(
            u => u.Playlists, p => p.VideoIds.Contains(id)), Builders<User>.Update.Pull("Playlists.$.VideoIds", id));
        await _tagRepo.DeleteManyAsync(t=>t.VideoId==id);
    }

    public async Task<VideoView> GetVideoViewAsync(string videoId,string userId)
    {
        var video = await GetByIdAsync(videoId);
        var subs = await _userService.GetFieldAsync(u => u.Id == ObjectId.Parse(userId), u => u.Subscriptions);
        var sub = subs.Where(s => s.ChannelId == video.ChannelId).FirstOrDefault();
        return DTOManager.FromVideoToView(video, sub,userId);
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
        await _repo.UpdateOneAsync(v => v.Id == ObjectId.Parse(videoId), update);
    }

    public async Task<double> RateVideo(string videoId,string userId,double rating=0.0)
    {
        if (Math.Abs(rating) > 5.0)
            throw new CustomException("Ratings can't exceed 5.");
        if(rating>0.0)
        {
            var oldRating = (await _repo.GetFieldAsync(v => v.Id == ObjectId.Parse(videoId), v => v.Ratings))
            .Where(r=>r.Item1==userId).FirstOrDefault();
            if(oldRating!=null)
                await _repo.RemoveFromListAsync(v => v.Id == ObjectId.Parse(videoId),
                v=>v.Ratings,oldRating);
            await _repo.InsertIntoListAsync(v=>v.Id==ObjectId.Parse(videoId),v=>v.Ratings,Tuple.Create(userId,rating));
        }
        return await _repo.UpdateRatingAsync(videoId);
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
        var vid =await _repo.IncrementViews(id);
    }

    public async Task<List<VideoCardView>> SearchForVideosAsync(string query)
    {
        var videos = await _repo.SearchForVideosAsync(query);
        return videos.Select(DTOManager.FromVideoToCardView).ToList();
    }

    public async Task<List<VideoCardView>> GetRecommendedVideosAsync(string id)
    {
        return (await _repo.GetRecommendedVideosAsync(id)).Select(DTOManager.FromVideoToCardView).ToList();
    }

    public async Task AddComment(Comment comment)
    {
        await _repo.AddComment(comment);
        await _userService.AddNotification(DTOManager.FromCommentToNotification(comment));
    }

    public async Task DeleteComment(string id)
    {
        await _repo.RemoveFromListAsync(Builders<Video>.Filter.ElemMatch(v =>
        v.Comments, c => c.Id == ObjectId.Parse(id)), v => v.Comments, c => c.Id == ObjectId.Parse(id));
    }

    public async Task LikeComment(string userId,string commentId,bool direction)
    {
        var update = direction ? Builders<Video>.Update.Push(
            "Comments.$.Likes", userId
        ) : Builders<Video>.Update.Pull("Comments.$.Likes", userId);

        await _repo.UpdateOneAsync(Builders<Video>.Filter.ElemMatch(v =>
        v.Comments, c => c.Id == ObjectId.Parse(commentId)),update);

       
    }
}