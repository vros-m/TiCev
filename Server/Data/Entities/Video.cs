namespace TiCev.Server.Data.Entities;
public class Video :MongoEntity
{
    public string ChannelId { get; set; } = null!;
    public string ChannelName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public long UploadDate { get; set; } = 0;
    public string ThumbnailId { get; set; } = "";
    public string VideoId { get; set; } = null!;
    public int Views { get; set; } = 0;
    public List<Tuple<string,double>> Ratings { get; set; } = [];
    public double Rating { get; set; } = 0;
    public List<Comment> Comments { get; set; } = [];
}

public class VideoDTO
{
    public string Title { get; set; } = null!;
    public string ChannelId { get; set; } = null!;
    public List<IFormFile> Files { get; set; } = null!;
    public string Description { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public string ChannelName { get; set; } = null!;

}

public record class VideoCardView(string ChannelId,string Title,int Views,double Rating, string ChannelName,string Id,string ThumbnailId);

public record class VideoView(string ChannelId,string Title, int Views,
 double Rating, double MyRating, string ChannelName,
string Id,string VideoId,bool IsSubscribed,string Description,List<string>Tags,List<CommentView> Comments);
