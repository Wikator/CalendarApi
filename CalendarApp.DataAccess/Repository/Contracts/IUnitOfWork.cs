using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ISubjectRepository SubjectRepository { get; }
    public ICrudRepository<Test> TestRepository { get; }
    public IScheduledClassRepository ScheduledClassRepository { get; }
    public IAuthorizedCrudRepository<Note> NoteRepository { get; }

    public Task<bool> SaveChangesAsync();
}