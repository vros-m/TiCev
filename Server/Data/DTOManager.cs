using MongoDB.Bson;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Data;

public static class DTOManager
{
    public static Video FromDTOToVideo(VideoDTO dto,ObjectId fileId,ObjectId? thumbnailId=null)
    {
        return new Video
        {
            UserId = dto.UserId,
            UploadDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Title = dto.Title,
            Description = dto.Description,
            ChannelName = dto.ChannelName,
            VideoId = fileId.ToString(),
            ThumbnailId = thumbnailId?.ToString() ?? ""
        };
    }
}