
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;
public class VideoRepo(IMongoClient client/*,*/) :ARepo<Video>(client,"videos")
{
    private GridFSBucket _bucket = new(client.GetDatabase(Constants.ConstObj.DatabaseName));

    public async Task<Video> UploadVideoAsync(VideoDTO dto)
    {
        using var stream = dto.File.OpenReadStream();
        ObjectId fileId=await _bucket.UploadFromStreamAsync(dto.File.FileName, stream);
        Video video = DTOManager.FromDTOToVideo(dto,fileId);
        
        return await AddAsync(video);
    }

    public async Task DeleteVideoAsync(string id)
    {
        var objectId = ObjectId.Parse(id);

        var video = (await GetByIdAsync(id))!;
        var task =  _bucket.DeleteAsync(ObjectId.Parse(video.VideoId));
        var task2 = DeleteAsync(video=>video.Id==objectId);
        await task;
        await task2;
    }

    public async Task<GridFSDownloadStream> GetVideoAsync(string id)
    {
        return await _bucket.OpenDownloadStreamAsync(ObjectId.Parse(id));
    }

}