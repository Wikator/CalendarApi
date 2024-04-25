using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IScheduledClassRepository
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>>? predicate = null);
    public Task<TDto?> GetByIdAsync<TDto>(int id, int? userId);
    public Task<ScheduledClass?> GetByIdAsync(int id, int? userId);
    public Task<ScheduledClass?> GetByIdAsync(int id);
    public void Add(ScheduledClass entity);
    public void Delete(ScheduledClass entity);
}