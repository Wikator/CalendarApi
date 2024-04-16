using System.Linq.Expressions;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IScheduledClassRepository
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(uint? userId,
        Expression<Func<ScheduledClass, bool>>? predicate = null);
    public Task<TDto?> GetByIdAsync<TDto>(uint id, uint? userId);
    public Task<ScheduledClass?> GetByIdAsync(uint id, uint? userId);
    public Task<ScheduledClass?> GetByIdAsync(uint id);
    public void Add(ScheduledClass entity);
    public void Delete(ScheduledClass entity);
}