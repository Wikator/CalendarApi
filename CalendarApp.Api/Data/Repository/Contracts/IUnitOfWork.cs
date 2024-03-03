using CalendarApp.Api.Entities;

namespace CalendarApp.Api.Data.Repository.Contracts;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ICrudRepository<Subject> SubjectRepository { get; }
    public ICrudRepository<ScheduledClass> ScheduledClassRepository { get; }

    public Task<bool> SaveChangesAsync();
}