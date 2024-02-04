
using System.Buffers.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;
public class VideoRepo(IMongoClient client/*,*/) :ARepo<Video>(client,"videos")
{
    private readonly GridFSBucket _bucket = new(client.GetDatabase(Constants.ConstObj.DatabaseName));


    public async Task<Video> UploadVideoAsync(VideoDTO dto)
    {
        using var stream = dto.Files[0].OpenReadStream();
        var thumbnail = dto.Files.Count == 2 ? dto.Files[1].OpenReadStream() : null;
        ObjectId fileId=await _bucket.UploadFromStreamAsync(dto.Files[0].FileName, stream,
        new GridFSUploadOptions{
            Metadata=new(new Dictionary<string,string>{
                {"contentType",dto.Files[0].ContentType}
            })
        });
        ObjectId? thumbnailId = thumbnail!=null?(await _bucket.
        UploadFromStreamAsync("thumbnail:" + dto.Files[0].FileName, thumbnail,
        new GridFSUploadOptions{
            Metadata=new BsonDocument(new Dictionary<string,string>{
                {"contentType",dto.Files[1].ContentType}
            })
        })):null;
        Video video = DTOManager.FromDTOToVideo(dto,fileId,thumbnailId);
    
        return await AddAsync(video);
    }

    public async Task DeleteVideoAsync(string id)
    {
        var objectId = ObjectId.Parse(id);

        var video = (await GetByIdAsync(id))!;
        var task =  _bucket.DeleteAsync(ObjectId.Parse(video.VideoId));
        var task2 = DeleteAsync(video=>video.Id==objectId);
        var task3 = video.ThumbnailId!=""?_bucket.DeleteAsync(ObjectId.Parse(video.ThumbnailId)):null;
        await task;
        await task2;
        if (task3 != null)
            await task3;
    }

    public async Task<GridFSDownloadStream> GetVideoAsync(string id)
    {
        return await _bucket.OpenDownloadStreamAsync(ObjectId.Parse(id));
    }

    public async Task<Tuple<byte[],string>> GetThumbnailAsync(string id)
    {
        var bytes = await _bucket.DownloadAsBytesAsync(ObjectId.Parse(id));
        var fileInfo = _bucket.Find(
            Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(id))
        ).FirstOrDefault();
        var mimeType = fileInfo.Metadata.GetValue("contentType").ToString();
        return Tuple.Create(bytes, mimeType!);
    }

    public async Task IncrementViews(string id)
    {
        await _collection.FindOneAndUpdateAsync(v => v.Id == ObjectId.Parse(id), Builders<Video>.Update.Inc(v => v.Views,1));
    }

}