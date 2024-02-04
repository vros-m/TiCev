using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TiCev.Server.Business.Repos;

public abstract class ARepo<T>(IMongoClient client,string collectionName)
{
    protected readonly IMongoClient _client = client;
    protected readonly string collectionName = collectionName;
    protected IMongoCollection<T> _collection
    =client.GetDatabase(Constants.ConstObj.DatabaseName).GetCollection<T>(collectionName);

    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(item => true).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var objId = ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq("_id", objId);
        return (await _collection.FindAsync(filter)).FirstOrDefault();
    } 

    public virtual async Task<T> AddAsync(T document)
    {
        await _collection.InsertOneAsync(document);
        return document;
    }

    public async Task UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(Expression<Func<T, bool>> filter)
    {
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<TField> GetFieldAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field)
    {
        var projection = Builders<T>.Projection.Expression(field);
        var result = await _collection.Find(filter).Project(projection).FirstOrDefaultAsync();

        return result;
    }
    public async Task InsertIntoListAsync<TItem>(
        Expression<Func<T, object>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        TItem item)
    {
        var filterDefinition = Builders<T>.Filter.Exists(filter);
        var updateDefinition = Builders<T>.Update.Push(listProperty, item);

        await _collection.UpdateOneAsync(filterDefinition, updateDefinition);
    }

     public async Task RemoveFromListAsync<TItem>(
        Expression<Func<T, object>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        Expression<Func<TItem, bool>> condition)
    {
        var filterDefinition = Builders<T>.Filter.Exists(filter);
        var update = Builders<T>.Update.PullFilter(listProperty, Builders<TItem>.Filter.Where(condition));

        await _collection.UpdateOneAsync(filterDefinition, update);
    }
}