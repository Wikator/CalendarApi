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

    public async Task<IEnumerable<TDto>> GetAllAsync<TDto>(uint? userId,
        Expression<Func<ScheduledClass, bool>>? predicate = null)
    {
        var query = Entities
            .Include(s => s.Notes)
            .Select(s => new ScheduledClass
            {
                Id = s.Id,
                SubjectId = s.SubjectId,
                Subject = s.Subject,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsCancelled = s.IsCancelled,
                Notes = s.Notes.Where(n => n.UserId == userId).ToList()
            });
        
        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ProjectTo<TDto>(Mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<TDto?> GetByIdAsync<TDto>(uint id, uint? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Include(s => s.Notes)
            .Select(s => new ScheduledClass
            {
                Id = s.Id,
                SubjectId = s.SubjectId,
                Subject = s.Subject,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsCancelled = s.IsCancelled,
                Notes = s.Notes.Where(n => n.UserId == userId).ToList()
            })
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<ScheduledClass?> GetByIdAsync(uint id, uint? userId)
    {
        return await Entities
            .Where(e => e.Id == id)
            .Include(s => s.Notes)
            .Select(s => new ScheduledClass
            {
                Id = s.Id,
                SubjectId = s.SubjectId,
                Subject = s.Subject,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsCancelled = s.IsCancelled,
                Notes = s.Notes.Where(n => n.UserId == userId).ToList()
            })
            .SingleOrDefaultAsync();
    }
    
    public async Task<ScheduledClass?> GetByIdAsync(uint id)
    {
        return await Entities
            .SingleOrDefaultAsync(e => e.Id == id);
    }

    public void Add(ScheduledClass entity)
    {
        Entities.Add(entity);
    }

    public void Delete(ScheduledClass entity)
    {
        Entities.Remove(entity);
    }
}