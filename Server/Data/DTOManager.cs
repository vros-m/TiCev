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
            ChannelName=dto.ChannelName,
            Tags=dto.Tags
        };
    }

    public static VideoCardView FromVideoToCardView(Video video)
    {
        return new VideoCardView(video.ChannelId, video.Title, video.Views, video.Rating, video.ChannelName, video.ObjectId);
    }

    public static SimpleUser SimplifyUser(User user)
    {
        return new SimpleUser(user.ObjectId,
            user.Username,user.Notifications);
         
    }

    public static VideoView FromVideoToView(Video video, Subscription? sub,string userId)
    {
        var rating = video.Ratings.Find(r => r.Item1 == userId)?.Item2 ?? 0.0;
        return new VideoView(video.ChannelId, video.Title, video.Views, video.Rating, rating, video.ChannelName,
        video.ObjectId, video.VideoId, sub != null, video.Description, video.Tags,video.Comments.Select(
            item=>FromCommentToView(item,userId)).ToList());
    }

    public static UserView FromDTOToUserView(UserViewDTO dto,string currentUserId)
    {
        return new UserView(dto.Username, dto.Email, dto.Bio, dto.Birthday, dto.Gender, dto.Videos.Select(FromVideoToCardView).ToList()
        , dto.Subscriptions, dto.Playlists, dto.Id.ToString(),dto.Subscribers.Contains(currentUserId));
    }

    public static CommentView FromCommentToView(Comment comment,string userId)
    {
        var IsLiked = comment.Likes.Find(id => id == userId)!=null;
        return new CommentView(comment.Id.Timestamp, comment.ObjectId, comment.ReplyingToId,
        comment.VideoId, comment.Likes.Count, IsLiked, comment.Text, comment.UserId, comment.Username, comment.ProfilePicture);
    }

    public static Notification FromVideoToNotification(Video vid, string userId)
    {
        return new Notification
        {
            Message=$"{vid.ChannelName} has posted a new video: \"{vid.Title}\"",
            SenderId=vid.ChannelId,
            RecipientId=userId,
            VideoId=vid.ObjectId,
        };
    }

    public static Notification FromCommentToNotification(Comment comment)
    {
        return new Notification
        {
            Message = $"New comment under your video: \"{comment.Text}\"",
            SenderId=comment.UserId,
            RecipientId=comment.VideoPosterId,
            VideoId=comment.VideoId
        };
    }
}