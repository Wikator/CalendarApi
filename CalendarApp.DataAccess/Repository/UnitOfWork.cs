using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class UnitOfWork(ApplicationDbContext dbContext, IMapper mapper) : IUnitOfWork
{
    private DbContext DbContext { get; } = dbContext;

    public IUserRepository UserRepository { get; } = new UserRepository(dbContext, mapper);
    public ICrudRepository<Subject> SubjectRepository { get; } = new CrudRepository<Subject>(dbContext, mapper);

    public IScheduledClassRepository ScheduledClassRepository { get; } =
        new ScheduledClassRepository(dbContext, mapper);
    
    public IAuthorizedCrudRepository<Note> NoteRepository { get; } = new AuthorizedCrudRepository<Note>(dbContext, mapper);

    public async Task<bool> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync() > 0;
    }
}