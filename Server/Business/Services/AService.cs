using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;
using TiCev.Server.Data.Entities;
using TiCev.Server.Error;

namespace TiCev.Server.Business.Services;

public abstract class AService<T>(ARepo<T> repo)
{
    protected readonly ARepo<T> _repo = repo;

    public virtual async Task<T?> GetFirstByFieldAsync(Expression<Func<T,bool>> filter)
    {
        return await _repo.GetFirstByFieldAsync(filter);
    }

     public virtual async Task<List<T>> GetAllByFieldAsync(Expression<Func<T,bool>> filter)
    {
        return await _repo.GetAllByFieldAsync(filter);
    }
    public virtual async Task<List<T>> GetAllAsync(int skip=0,int limit=0x7FFFFFFF)
    {
        return await _repo.GetAllAsync(skip,limit);
    }

    public virtual async Task<T> GetByIdAsync(string id)
    {
        var item = await _repo.GetByIdAsync(id) ?? throw new CustomException($"Document with id {id} doesn't exist.");
        return item!;
    }

    public virtual async Task<T> AddAsync(T document)
    {
        return await _repo.AddAsync(document);
    }
    public virtual async Task UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _repo.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _repo.UpdateManyAsync(filter, update);
    }

    public virtual async Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        await _repo.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        await _repo.UpdateManyAsync(filter, update);
    }
    public virtual async Task DeleteAsync(Expression<Func<T, bool>> filter)
    {
        await _repo.DeleteAsync(filter);
    }

    public virtual async Task<TField> GetFieldAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field)
    {
        return await _repo.GetFieldAsync(filter, field);
    }
    public virtual async Task InsertIntoListAsync<TItem>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        TItem item)
    {
       await _repo.InsertIntoListAsync(filter,listProperty,item);
    }

     public virtual async Task RemoveFromListAsync<TItem>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        Expression<Func<TItem, bool>> condition) where TItem:class
    {
        await _repo.RemoveFromListAsync(filter, listProperty, condition);
    }

     public virtual async Task RemoveFromListAsync<TItem>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        TItem value)
    {
        await _repo.RemoveFromListAsync(filter, listProperty, value);
    }
}