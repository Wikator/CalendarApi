using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class CrudRepository<T>(DbContext context, IMapper mapper) : ICrudRepository<T> where T : class, IEntity
{
    private IMapper Mapper { get; } = mapper;
    private DbSet<T> Entities { get; } = context.Set<T>();

    public async Task<IEnumerable<TDto>> GetAllAsync<TDto>(Expression<Func<T, bool>>? predicate = null)
    {
        var query = Entities.AsQueryable();

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ProjectTo<TDto>(Mapper.ConfigurationProvider).ToListAsync();
    }

    public Task<TDto?> GetByIdAsync<TDto>(uint id)
    {
        return Entities
            .Where(e => e.Id == id)
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(uint id)
    {
        return await Entities.FindAsync(id);
    }

    public void Add(T entity)
    {
        Entities.Add(entity);
    }

    public void Delete(T entity)
    {
        Entities.Remove(entity);
    }
}