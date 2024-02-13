using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Repos;

public abstract class ARepo<T>(IMongoClient client,string collectionName)
{
    protected readonly IMongoClient _client = client;
    protected readonly string collectionName = collectionName;
    protected IMongoCollection<T> _collection
    =client.GetDatabase(Constants.ConstObj.DatabaseName).GetCollection<T>(collectionName);

    public virtual async Task<List<T>> GetAllAsync(int skip=0,int limit=0x7FFFFFFF,SortDefinition<T>? sortDefinition=null)
    {
        sortDefinition ??= new BsonDocument("_id", -1);
        return await _collection.Find(item => true).Sort(sortDefinition).Skip(skip).Limit(limit).ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(string id)
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

    public virtual async Task<IEnumerable<T>> AddManyAsync(IEnumerable<T> documents)
    {
        await _collection.InsertManyAsync(documents);
        return documents;
    }

    public virtual async Task UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateManyAsync(filter, update);
    }

    public virtual async Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateManyAsync(filter, update);
    }
    public virtual async Task DeleteAsync(Expression<Func<T, bool>> filter)
    {
        await _collection.DeleteOneAsync(filter);
    }
    public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> filter)
    {
        await _collection.DeleteManyAsync(filter);
    }

    public virtual async Task<TField> GetFieldAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field)
    {
        var projection = Builders<T>.Projection.Expression(field);
        var result = await _collection.Find(filter).Project(projection).FirstOrDefaultAsync();

        return result;
    }
    public virtual async Task InsertIntoListAsync<TItem>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        TItem item)
    {
        var updateDefinition = Builders<T>.Update.Push(listProperty, item);

        await UpdateOneAsync(filter, updateDefinition);
    }


    /// <summary>
    /// Method for removing objects from arrays (use the overloaded version for strings)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="filter"></param>
    /// <param name="listProperty"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
     public virtual async Task RemoveFromListAsync<TItem>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        Expression<Func<TItem, bool>> condition) where TItem:class
    {
        var update = Builders<T>.Update.PullFilter(listProperty, Builders<TItem>.Filter.Where(condition));

        await UpdateOneAsync(filter, update);
    }



        /// <summary>
    /// Method for removing objects from arrays (use the overloaded version for strings)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="filter"></param>
    /// <param name="listProperty"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public virtual async Task RemoveFromListAsync<TItem>(
        FilterDefinition<T> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        Expression<Func<TItem, bool>> condition) where TItem:class
    {
        var update = Builders<T>.Update.PullFilter(listProperty, Builders<TItem>.Filter.Where(condition));

        await UpdateOneAsync(filter, update);
    }
    public virtual async Task RemoveFromListAsync<TItem>(Expression<Func<T, bool>> filter,
    Expression<Func<T, IEnumerable<TItem>>> listProperty,TItem value)
    {
        var update = Builders<T>.Update.Pull(listProperty, value);
        await UpdateOneAsync(filter, update);
    }

    public virtual async Task<T?> GetFirstByFieldAsync(Expression<Func<T, bool>> filter)
    {
        return (await _collection.FindAsync(filter)).FirstOrDefault();
    }

        public virtual async Task<T?> GetFirstByFieldAsync(FilterDefinition<T> filter)
    {
        return (await _collection.FindAsync(filter)).FirstOrDefault();
    }

    public virtual async Task<List<T>> GetAllByFieldAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }
}