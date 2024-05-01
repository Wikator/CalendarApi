using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public class UnitOfWork(ApplicationDbContext dbContext, IMapper mapper) : IUnitOfWork
{
    public IUserRepository UserRepository { get; } = new UserRepository(dbContext, mapper);
    public ISubjectRepository SubjectRepository { get; } = new SubjectRepository(dbContext, mapper);
    public ICrudRepository<Test> TestRepository { get; } = new CrudRepository<Test>(dbContext, mapper);

    public IScheduledClassRepository ScheduledClassRepository { get; } =
        new ScheduledClassRepository(dbContext, mapper);
    
    public IAuthorizedCrudRepository<Note> NoteRepository { get; } = new AuthorizedCrudRepository<Note>(dbContext, mapper);

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}