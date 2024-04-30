using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class ScheduledClassRepository(DbContext context, IMapper mapper) : IScheduledClassRepository
{
    private IMapper Mapper { get; } = mapper;
    private DbSet<ScheduledClass> Entities { get; } = context.Set<ScheduledClass>();

    public async Task<IEnumerable<TDto>> GetAllAsync<TDto>(int? userId,
        Expression<Func<ScheduledClass, bool>>? predicate = null)
    {
        var query = Entities
            .Include(s => s.Notes)
            .Include(s => s.Subject)
            .AsQueryable();
        
        if (predicate is not null)
            query = query.Where(predicate);

        if (userId is not null)
            query = await GetUserScheduledClasses(query, userId.Value);
        
        return await query
            .Select(ExcludeNonUserNotes(userId))
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TDto?> GetByIdAsync<TDto>(int id, int? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Include(s => s.Notes)
            .Select(ExcludeNonUserNotes(userId))
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<ScheduledClass?> GetByIdAsync(int id, int? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Include(s => s.Notes)
            .Select(ExcludeNonUserNotes(userId))
            .SingleOrDefaultAsync();
    }

    public async Task<ScheduledClass?> GetByIdAsync(int id)
    {
        return await Entities.SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<ScheduledClass, bool>> predicate)
    {
        return await Entities.AnyAsync(predicate);
    }

    public void Add(ScheduledClass entity)
    {
        Entities.Add(entity);
    }

    public void Update(ScheduledClass entity)
    {
        Entities.Update(entity);
    }

    public void Delete(ScheduledClass entity)
    {
        Entities.Remove(entity);
    }

    private async Task<IQueryable<ScheduledClass>> GetUserScheduledClasses(IQueryable<ScheduledClass> query,
        int userId)
    {
        var user = await context.Set<User>()
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