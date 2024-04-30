using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class AuthorizedCrudRepository<T>(DbContext context, IMapper mapper) :
    IAuthorizedCrudRepository<T> where T : class, IAuthorizedEntity
{
    protected IConfigurationProvider MapperConfiguration { get; } = mapper.ConfigurationProvider;
    protected DbSet<T> Entities { get; } = context.Set<T>();

    public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>(int id, Expression<Func<T, bool>>? predicate = null)
    {
        var query = Entities.Where(e => e.UserId == id);

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ProjectTo<TDto>(MapperConfiguration).ToListAsync();
    }

    public virtual async Task<TDto?> GetByIdAsync<TDto>(int id, int userId)
    {
        return await Entities
            .Where(e => e.Id == id && e.UserId == userId)
            .ProjectTo<TDto>(MapperConfiguration)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id, int userId)
    {
        return await Entities
            .Where(e => e.UserId == userId && e.Id == id)
            .SingleOrDefaultAsync();
    }

    public virtual void Add(T entity, int userId)
    {
        entity.UserId = userId;
        Entities.Add(entity);
    }

    public virtual async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        var entity = await GetByIdAsync(id, userId);

        if (entity is null || entity.UserId != userId)
            return false;

        Entities.Remove(entity);
        return true;
    }
}
