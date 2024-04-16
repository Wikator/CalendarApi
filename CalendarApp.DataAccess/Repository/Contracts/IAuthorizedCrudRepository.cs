using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IAuthorizedCrudRepository<T> where T : class, IAuthorizedEntity
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(uint userId, Expression<Func<T, bool>>? predicate = null);
    public Task<TDto?> GetByIdAsync<TDto>(uint userId, uint id);
    public Task<T?> GetByIdAsync(uint userId, uint id);
    public void Add(T entity, uint userId);
    public void Delete(T entity, uint userId);
}