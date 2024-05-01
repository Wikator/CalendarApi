using System.Linq.Expressions;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IScheduledClassRepository : ICrudRepository<ScheduledClass>
{
    public Task<IEnumerable<TDto>> GetAllWithUserNotesAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>> predicate);
    public Task<IEnumerable<TDto>> GetAllWithUserNotesAsync<TDto>(int? userId);
    public Task<TDto?> GetByIdWithUserNotesAsync<TDto>(int id, int? userId);
    public Task<ScheduledClass?> GetByIdWithUserNotesAsync(int id, int? userId);
    public void Update(ScheduledClass scheduledClass);
}