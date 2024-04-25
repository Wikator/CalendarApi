using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IAuthorizedCrudRepository<T> where T : class, IAuthorizedEntity
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(int userId, Expression<Func<T, bool>>? predicate = null);
    public Task<TDto?> GetByIdAsync<TDto>(int userId, int id);
    public Task<T?> GetByIdAsync(int userId, int id);
    public void Add(T entity, int userId);
    public Task<bool> DeleteByIdAsync(int id, int userId);
}