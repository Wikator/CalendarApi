using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface ISubjectRepository : ICrudRepository<Subject>
{
    public Task<IEnumerable<TDto>> GetAllAsync<TDto>(int group);
    public Task<TDto?> GetByIdAsync<TDto>(int id, int group);
}
