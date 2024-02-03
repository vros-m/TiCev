using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;
using TiCev.Server.Error;

namespace TiCev.Server.Business.Services;

public abstract class AService<T>(ARepo<T> repo)
{
    protected readonly ARepo<T> _repo = repo;

    public async Task<List<T>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var item = await _repo.GetByIdAsync(id) ?? throw new CustomException($"Document with id {id} doesn't exist.");
        return item!;
    }

    public async Task UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _repo.UpdateAsync(filter, update);
    }

    public async Task DeleteAsync(Expression<Func<T, bool>> filter)
    {
        await _repo.DeleteAsync(filter);
    }

    public async Task<TField> GetFieldAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field)
    {
        return await _repo.GetFieldAsync(filter, field);
    }
    public async Task InsertIntoListAsync<TItem>(
        Expression<Func<T, object>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        TItem item)
    {
       await _repo.InsertIntoListAsync(filter,listProperty,item);
    }

     public async Task RemoveFromListAsync<TItem>(
        Expression<Func<T, object>> filter,
        Expression<Func<T, IEnumerable<TItem>>> listProperty,
        Expression<Func<TItem, bool>> condition)
    {
        await _repo.RemoveFromListAsync(filter, listProperty, condition);
    }
}