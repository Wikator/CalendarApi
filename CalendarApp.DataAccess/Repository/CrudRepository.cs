using System.Linq.Expressions;
using AutoMapper;
using CalendarApp.DataAccess.Extensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class CrudRepository<T>(DbContext context, IMapper mapper) : ICrudRepository<T> where T : class, IEntity
{
    protected IConfigurationProvider MapperConfiguration { get; } = mapper.ConfigurationProvider;
    protected DbSet<T> Entities { get; } = context.Set<T>();

    public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>(Expression<Func<T, bool>> predicate)
    {
        return await Entities
            .ToListProjectedAsync<T, TDto>(predicate, MapperConfiguration);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>()
    {
        return await Entities
            .ToListProjectedAsync<T, TDto>(MapperConfiguration);
    }

    public virtual async Task<TDto?> GetByIdAsync<TDto>(int id)
    {
        return await Entities
            .SingleOrDefaultProjectedAsync<T, TDto>(e => e.Id == id, MapperConfiguration);
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await Entities.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(int id, Expression<Func<T, object?>> include)
    {
        return await Entities.Include(include).SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await Entities.AnyAsync(predicate);
    }

    public virtual void Add(T entity)
    {
        Entities.Add(entity);
    }

    public virtual void Delete(T entity)
    {
        Entities.Remove(entity);
    }
}
