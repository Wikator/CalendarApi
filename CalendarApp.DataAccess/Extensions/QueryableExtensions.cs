using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Extensions;

public static class QueryableExtensions
{
    public static async Task<IEnumerable<TDto>> ToListProjectedAsync<T, TDto>(this IQueryable<T> queryable,
        IConfigurationProvider mapperConfiguration)
    {
        return await queryable
            .ProjectTo<TDto>(mapperConfiguration)
            .ToListAsync();
    }
    
    public static async Task<IEnumerable<TDto>> ToListProjectedAsync<T, TDto>(this IQueryable<T> queryable,
        Expression<Func<T, bool>> predicate, IConfigurationProvider mapperConfiguration)
    {
        return await queryable
            .Where(predicate)
            .ProjectTo<TDto>(mapperConfiguration)
            .ToListAsync();
    }
    
    public static async Task<TDto?> SingleOrDefaultProjectedAsync<T, TDto>(this IQueryable<T> queryable,
        Expression<Func<T, bool>> predicate, IConfigurationProvider mapperConfiguration)
    {
        return await queryable
            .Where(predicate)
            .ProjectTo<TDto>(mapperConfiguration)
            .SingleOrDefaultAsync();
    }
}
