using MongoDB.Bson;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Data;

public static class DTOManager
{
    public static Video FromDTOToVideo(VideoDTO dto,ObjectId fileId,ObjectId? thumbnailId=null)
    {
        return new Video
        {
            ChannelId = dto.ChannelId,
            UploadDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Title = dto.Title,
            Description = dto.Description,
            VideoId = fileId.ToString(),
            ThumbnailId = thumbnailId?.ToString() ?? "",
            ChannelName=dto.ChannelName
        };
    }

    public static VideoCardView FromVideoToCardView(Video video)
    {
        return new VideoCardView(video.ChannelId, video.Title, video.Views, video.Rating, video.ChannelName, video.ObjectId);
    }

    public static SimpleUser SimplifyUser(User user)
    {
        return new SimpleUser(user.ObjectId,
            user.Username);
         
    }

    public static VideoView FromVideoToView(Video video, Subscription? sub,string userId)
    {
        var rating = video.Ratings.Find(r => r.Item1 == userId)?.Item2 ?? 0.0;
        return new VideoView(video.ChannelId, video.Title, video.Views, video.Rating, rating, video.ChannelName,
        video.ObjectId, video.VideoId, sub != null, video.Description, video.Tags);
    }

    public static UserView FromDTOToUserView(UserViewDTO dto,string currentUserId)
    {
        return new UserView(dto.Username, dto.Email, dto.Bio, dto.Birthday, dto.Gender, dto.Videos.Select(FromVideoToCardView).ToList()
        , dto.Subscriptions, dto.Playlists, dto.Id.ToString(),dto.Subscribers.Contains(currentUserId));
    }

}