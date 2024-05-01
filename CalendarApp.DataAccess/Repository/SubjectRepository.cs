using System.Linq.Expressions;
using AutoMapper;
using CalendarApp.DataAccess.Extensions;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public sealed class SubjectRepository(
    DbContext context,
    IMapper mapper) : CrudRepository<Subject>(context, mapper), ISubjectRepository
{
    public async Task<IEnumerable<TDto>> GetAllWithUserTestsAsync<TDto>(int group)
    {
        return await Entities
            .Select(ExcludeUserIrrelevantTests(group))
            .ToListProjectedAsync<Subject, TDto>(MapperConfiguration);
    }

    public async Task<TDto?> GetByIdWithUserTestsAsync<TDto>(int id, int group)
    {
        return await Entities
            .Select(ExcludeUserIrrelevantTests(group))
            .SingleOrDefaultProjectedAsync<Subject, TDto>(s => s.Id == id, MapperConfiguration);
    }

    private static Expression<Func<Subject, Subject>> ExcludeUserIrrelevantTests(int group) =>
        s => new Subject
        {
            FacultyType = s.FacultyType,
            Id = s.Id,
            Name = s.Name,
            Tests = s.Tests.Where(t => t.Group == null || t.Group == group).ToList()
        };
}