using MongoDB.Driver;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;

public class TagRepo(IMongoClient client):ARepo<Data.Entities.Tag>(client,"tags")
{
    
}