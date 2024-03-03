using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ICrudRepository<Subject> SubjectRepository { get; }
    public ICrudRepository<ScheduledClass> ScheduledClassRepository { get; }

    public Task<bool> SaveChangesAsync();
}