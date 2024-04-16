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
    protected IMapper Mapper { get; } = mapper;
    protected DbSet<T> Entities { get; } = context.Set<T>();

    public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>(uint id, Expression<Func<T, bool>>? predicate = null)
    {
        var query = Entities.Where(e => e.UserId == id);

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ProjectTo<TDto>(Mapper.ConfigurationProvider).ToListAsync();
    }

    public virtual async Task<TDto?> GetByIdAsync<TDto>(uint id, uint userId)
    {
        return await Entities
            .Where(e => e.Id == id && e.UserId == userId)
            .ProjectTo<TDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<T?> GetByIdAsync(uint id, uint userId)
    {
        return await Entities
            .Where(e => e.UserId == userId && e.Id == id)
            .SingleOrDefaultAsync();
    }

    public virtual void Add(T entity, uint userId)
    {
        entity.UserId = userId;
        Entities.Add(entity);
    }

    public virtual void Delete(T entity, uint userId)
    {
        if (userId == entity.UserId)
            Entities.Remove(entity);
    }
}
