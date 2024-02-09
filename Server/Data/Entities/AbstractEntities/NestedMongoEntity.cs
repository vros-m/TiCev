namespace TiCev.Server.Data.Entities;

public abstract class NestedMongoEntity:MongoEntity
{
    public NestedMongoEntity()
    {
        Id = MongoDB.Bson.ObjectId.GenerateNewId();
    }
}