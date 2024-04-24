using AutoMapper;
using CalendarApp.Api.Configuration;
using CalendarApp.DataAccess;
using CalendarApp.DataAccess.Repository;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CalendarApp.Tests.RepositoryTests;

public class CrudRepositoryTests
{
    private ApplicationDbContext Context { get; }
    private IMapper Mapper { get; }

    public CrudRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<AutoMapperProfiles>());

        Mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetAllSubjects_ShouldReturnDtos()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subjects = new List<Subject>
        {
            new() { Name = "TestSubject1", FacultyType = 1 },
            new() { Name = "TestSubject2", FacultyType = 2 },
            new() { Name = "TestSubject3", FacultyType = 3 }
        };

        Context.Subjects.AddRange(subjects);
        await Context.SaveChangesAsync();

        var expectedDtos = subjects.Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            FacultyType = s.FacultyType
        });

        // Act
        var dtos = await subjectRepository.GetAllAsync<SubjectDto>();

        // Assert
        dtos.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetAllSubjects_WithPredicate_ShouldReturnDtos()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subjects = new List<Subject>
        {
            new() { Name = "TestSubject1", FacultyType = 1 },
            new() { Name = "TestSubject2", FacultyType = 2 },
            new() { Name = "TestSubject3", FacultyType = 3 }
        };

        Context.Subjects.AddRange(subjects);
        await Context.SaveChangesAsync();

        var expectedDtos = subjects.Where(s => s.Name != subjects[2].Name).Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            FacultyType = s.FacultyType
        });

        // Act
        var dtos = await subjectRepository.GetAllAsync<SubjectDto>(s => s.Name != subjects[2].Name);

        // Assert
        dtos.Should().BeEquivalentTo(expectedDtos);
    }
    
    [Fact]
    public async Task GetByIdSubject_ShouldReturnDto_WhenIdExists()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);
        await Context.SaveChangesAsync();

        var expectedDto = new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            FacultyType = subject.FacultyType
        };

        // Act
        var dto = await subjectRepository.GetByIdAsync<SubjectDto>(subject.Id);

        // Assert
        dto.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetByIdSubject_ShouldReturnNull_IdDoesNotExist()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        // Act
        var dto = await subjectRepository.GetByIdAsync<SubjectDto>(1);

        // Assert
        dto.Should().BeNull();
    }
    
    [Fact]
    public async Task GetByIdSubject_ShouldReturnEntity_IfExists()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);
        await Context.SaveChangesAsync();

        // Act
        var entity = await subjectRepository.GetByIdAsync(subject.Id);

        // Assert
        entity.Should().BeEquivalentTo(subject);
    }

    [Fact]
    public async Task GetByIdSubject_ShouldReturnNull_IfDoesNotExist()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        // Act
        var entity = await subjectRepository.GetByIdAsync(1);

        // Assert
        entity.Should().BeNull();
    }

    [Fact]
    public async Task AddSubject_ShouldAddEntity()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        // Act
        subjectRepository.Add(subject);
        await Context.SaveChangesAsync();

        // Assert
        Context.Subjects.Should().Contain(subject);
    }

    [Fact]
    public async Task DeleteSubject_ShouldDeleteEntity()
    {
        // Arrange
        var subjectRepository = new CrudRepository<Subject>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);
        await Context.SaveChangesAsync();

        // Act
        subjectRepository.Delete(subject);
        await Context.SaveChangesAsync();

        // Assert
        Context.Subjects.Should().NotContain(subject);
    }
}