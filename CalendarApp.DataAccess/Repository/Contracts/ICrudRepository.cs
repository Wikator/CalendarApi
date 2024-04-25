using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface ICrudRepository<T> where T : class, IEntity
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(Expression<Func<T, bool>>? predicate = null);
    public Task<TDto?> GetByIdAsync<TDto>(int id);
    public Task<T?> GetByIdAsync(int id);
    public void Add(T entity);
    public void Delete(T entity);
}