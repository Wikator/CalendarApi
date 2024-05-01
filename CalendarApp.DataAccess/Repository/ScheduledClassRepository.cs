using System.Linq.Expressions;
using AutoMapper;
using CalendarApp.DataAccess.Extensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public sealed class ScheduledClassRepository(DbContext context,
    IMapper mapper) : CrudRepository<ScheduledClass>(context, mapper), IScheduledClassRepository
{
    private readonly DbContext _context = context;

    public async Task<IEnumerable<TDto>> GetAllWithUserNotesAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>> predicate)
    {
        var query = Entities.Where(predicate);
        return await GetProjectedUserScheduledClasses<TDto>(query, userId);
    }

    public async Task<IEnumerable<TDto>> GetAllWithUserNotesAsync<TDto>(int? userId)
    {
        var query = Entities.AsQueryable();
        return await GetProjectedUserScheduledClasses<TDto>(query, userId);
    }

    public async Task<TDto?> GetByIdAsync<TDto>(int id, int? userId)
    {
        return await Entities
            .Select(ExcludeNonUserNotes(userId))
            .SingleOrDefaultProjectedAsync<ScheduledClass, TDto>(e => e.Id == id, MapperConfiguration);
    }

    public async Task<ScheduledClass?> GetByIdAsync(int id, int? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Select(ExcludeNonUserNotes(userId))
            .SingleOrDefaultAsync();
    }

    public void Update(ScheduledClass scheduledClass)
    {
        Entities.Update(scheduledClass);
    }

    private async Task<IQueryable<ScheduledClass>> GetUserScheduledClasses(IQueryable<ScheduledClass> query,
        int userId)
    {
        var user = await _context.Set<User>()
            .FindAsync(userId);
        
        return query.Where(s => s.Group == user!.Group &&
                                (s.Subject!.FacultyType == 0 ||
                                 s.SubjectId == user.Faculty1Id ||
                                 s.SubjectId == user.Faculty2Id ||
                                 s.SubjectId == user.Faculty3Id));
    }

    private static Expression<Func<ScheduledClass, ScheduledClass>> ExcludeNonUserNotes(int? userId) =>
        s => new ScheduledClass
        {
            Id = s.Id,
            SubjectId = s.SubjectId,
            Subject = s.Subject,
            Group = s.Group,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            IsCancelled = s.IsCancelled,
            Notes = s.Notes.Where(n => n.UserId == userId).ToList()
        };
    
    private async Task<IEnumerable<TDto>> GetProjectedUserScheduledClasses<TDto>(
        IQueryable<ScheduledClass> query, int? userId)
    {
        if (userId is not null)
            query = await GetUserScheduledClasses(query, userId.Value);
        
        return await query
            .Select(ExcludeNonUserNotes(userId))
            .ToListProjectedAsync<ScheduledClass, TDto>(MapperConfiguration);
    }
}