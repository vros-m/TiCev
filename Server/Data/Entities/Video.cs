namespace TiCev.Server.Data.Entities;

using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Video :MongoEntity
{
    public string UserId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = "";

    //public List<string> Tags { get; set; } = [];

    public long UploadDate { get; set; } = 0;
    public string ThumbnailId { get; set; } = "";
    public string VideoId { get; set; } = null!;
    public int Views { get; set; } = 0;
    public List<Tuple<string,double>> Ratings { get; set; } = [];

    public string ChannelName { get; set; } = null!;
}

public class VideoDTO
{
    public string Title { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public List<IFormFile> Files { get; set; } = null!;
    public string Description { get; set; } = "";
    public string ChannelName { get; set; } = null!;

}
