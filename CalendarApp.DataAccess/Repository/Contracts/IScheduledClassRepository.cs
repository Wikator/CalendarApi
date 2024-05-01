using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IScheduledClassRepository : ICrudRepository<ScheduledClass>
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>> predicate);
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId);
    public Task<TDto?> GetByIdAsync<TDto>(int id, int? userId);
    public Task<ScheduledClass?> GetByIdAsync(int id, int? userId);
    public void Update(ScheduledClass scheduledClass);
}