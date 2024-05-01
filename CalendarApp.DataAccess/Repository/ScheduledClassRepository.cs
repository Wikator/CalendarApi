using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class ScheduledClassRepository(DbContext context,
    IMapper mapper) : CrudRepository<ScheduledClass>(context, mapper), IScheduledClassRepository
{
    private readonly DbContext _context = context;

    public async Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>> predicate)
    {
        var query = Entities
            .Where(predicate);

        if (userId is not null)
            query = await GetUserScheduledClasses(query, userId.Value);
        
        return await query
            .Select(ExcludeNonUserNotes(userId))
            .ProjectTo<TDto>(MapperConfiguration)
            .ToListAsync();
    }

    public async Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId)
    {
        var query = Entities
            .AsQueryable();

        if (userId is not null)
            query = await GetUserScheduledClasses(query, userId.Value);
        
        return await query
            .Select(ExcludeNonUserNotes(userId))
            .ProjectTo<TDto>(MapperConfiguration)
            .ToListAsync();
    }

    public async Task<TDto?> GetByIdAsync<TDto>(int id, int? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Select(ExcludeNonUserNotes(userId))
            .ProjectTo<TDto>(MapperConfiguration)
            .SingleOrDefaultAsync();
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
}