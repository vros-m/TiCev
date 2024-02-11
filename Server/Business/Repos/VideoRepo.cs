
using System.Buffers.Text;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;
public class VideoRepo(IMongoClient client/*,*/) :ARepo<Video>(client,"videos")
{
    private readonly GridFSBucket _bucket = new(client.GetDatabase(Constants.ConstObj.DatabaseName));


    public async override Task<Video?> GetByIdAsync(string id)
    {
        BsonDocument[] pipeline = [
            new("$match",new BsonDocument("_id",ObjectId.Parse(id))),
            new("$unwind",new BsonDocument{
                {"path","$Comments"},
                {"preserveNullAndEmptyArrays",true} }),
            new("$lookup",new BsonDocument{
                {"from","users"},
                {"localField","Comments.UserId"},
                {"foreignField","_id"},
                {"as","User"}
            }),
             new("$unwind",new BsonDocument{
                {"path","$User"},
                {"preserveNullAndEmptyArrays",true} }),
            new("$addFields",new BsonDocument{
               {"Comments.Username","$User.Username"},
               {"Comments.ProfilePicture","$User.ProfilePicture"}
            }),
            new("$sort",new BsonDocument("Comments._id",-1)),
            new("$group",new BsonDocument{
                {"_id","$_id"},
                {"OriginalFields",new BsonDocument("$first","$$ROOT")},
                {"Comments",new BsonDocument("$push","$Comments")}
            }),
            new("$addFields",new BsonDocument("OriginalFields.Comments","$Comments")),
            new("$replaceRoot",new BsonDocument("newRoot","$OriginalFields")),
            new("$project",new BsonDocument{
                {"User",0}
            })
        ];
        var vid = (await _collection.AggregateAsync<Video>(pipeline)).First();
        vid.Comments = vid.Comments.Where(c => c.Text != null).ToList();
        return vid;
    }
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

    public async Task<Video> DeleteVideoAsync(string id)
    {
        var objectId = ObjectId.Parse(id);

        var video = (await GetByIdAsync(id))!;
        var task =  _bucket.DeleteAsync(ObjectId.Parse(video.VideoId));
        var task2 = base.DeleteAsync(video=>video.Id==objectId);
        var task3 = video.ThumbnailId!=""?_bucket.DeleteAsync(ObjectId.Parse(video.ThumbnailId)):null;
        await task;
        await task2;
        if (task3 != null)
            await task3;
        return video;
    }

/// <summary>
/// [deprecated]
/// </summary>
/// <param name="filter"></param>
/// <returns></returns>
/// <exception cref="NotImplementedException"></exception>
    public override Task DeleteAsync(Expression<Func<Video, bool>> filter)
    {
        throw new NotImplementedException();
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

    public async Task<Video> IncrementViews(string id)
    {
        return await _collection.FindOneAndUpdateAsync(v => v.Id == ObjectId.Parse(id), Builders<Video>.Update.Inc(v => v.Views,1));
    }

    public async Task UpdateRatingAsync(string videoId)
    {
        var id = ObjectId.Parse(videoId);
        var pipeline = new List<IPipelineStageDefinition>
        {
            PipelineStageDefinitionBuilder.Match<Video>(v=>v.Id==id),
            PipelineStageDefinitionBuilder.Unwind<Video>("Ratings"),
            PipelineStageDefinitionBuilder.Group<BsonDocument>(
                new BsonDocument{
                    {"_id", BsonNull.Value},
                    {"Rating",
                    new BsonDocument("$avg",new BsonDocument("$arrayElemAt",new BsonArray{"$Ratings",1}))}
                    })
        };

        var result = await _collection.AggregateAsync<BsonDocument>(pipeline);
        await UpdateOneAsync(v => v.Id == id, Builders<Video>.Update.Set("Rating", result.First().GetValue("rating")));
        
    }

    public async Task<List<Video>> SearchForVideosAsync(string query)
    {
        return (await _collection.FindAsync(Builders<Video>.Filter.Regex("Title", query))).ToList();
    }

    public async Task<List<Video>> GetRecommendedVideosAsync(string id)
    {
        BsonDocument[] pipeline = [
            new("$match",new BsonDocument("_id",ObjectId.Parse(id))),
            new("$lookup",new BsonDocument{
                { "from", "tags" },
                { "localField", "Tags" },
                { "foreignField", "TagName" },
                { "as", "JoinedTags" }
            }),
            new("$unwind","$JoinedTags"),
            new("$replaceRoot",new BsonDocument("newRoot","$JoinedTags")),
            new("$group",new BsonDocument{
                {"_id","$VideoId"},
                {"Tags",new BsonDocument("$addToSet","$TagName")},
                {"TagCount",new BsonDocument("$sum",1)}
            }),
            new("$lookup",new BsonDocument{
                {"from","videos"},
                { "localField", "_id"},
                {"foreignField", "_id"},
                {"as", "Videos"}
            }),new("$unwind","$Videos"),
            new("$sort",new BsonDocument("TagCount",-1)),
            new("$replaceRoot",new BsonDocument("newRoot","$Videos")),
            new("$match",new BsonDocument("_id",new BsonDocument("$not",new BsonDocument("$eq",ObjectId.Parse(id))))),
        ];
        return (await _collection.AggregateAsync<Video>(pipeline)).ToList();
    }

    public async Task<Comment> AddComment(Comment comment)
    {
        await InsertIntoListAsync(v => v.Id == ObjectId.Parse(comment.VideoId), v => v.Comments,
            comment);
        return comment;
        /* using var session = await client.StartSessionAsync();   
        session.StartTransaction();

        try
        {
            // Step 1: Insert a comment into the comments array
            await InsertIntoListAsync(v => v.Id == ObjectId.Parse(comment.VideoId), v => v.Comments,
            comment);

            // Step 2: Update the comment document with localId equal to the element count
            var localIdUpdate = Builders<Video>.Update.Set("Comments.$.LocalId",
                new BsonDocument("$size", "$Comments"));

            var localIdUpdateResult = await _collection.UpdateOneAsync(session,
                Builders<Video>.Filter.And(
                    Builders<Video>.Filter.Eq("_id", ObjectId.Parse(comment.VideoId)),
                    Builders<Video>.Filter.Eq("Comments._id", comment.Id)),
                localIdUpdate);

            await session.CommitTransactionAsync();
            return comment;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }  */         
    }



}
       
