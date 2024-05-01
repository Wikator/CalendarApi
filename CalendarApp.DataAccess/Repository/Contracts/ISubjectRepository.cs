using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface ISubjectRepository : ICrudRepository<Subject>
{
    public Task<IEnumerable<TDto>> GetAllWithUserTestsAsync<TDto>(int group);
    public Task<TDto?> GetByIdWithUserTestsAsync<TDto>(int id, int group);
}
