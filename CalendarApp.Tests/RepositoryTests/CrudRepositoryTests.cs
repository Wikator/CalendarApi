using AutoMapper;
using CalendarApp.Api.Configuration;
using CalendarApp.Api.Data;
using CalendarApp.Api.Data.Repository;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

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
    public async Task GetAllScheduledClasses_ShouldReturnDtos()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subjects = new List<Subject>
        {
            new() { Name = "TestSubject1", FacultyType = 1 },
            new() { Name = "TestSubject2", FacultyType = 2 },
            new() { Name = "TestSubject3", FacultyType = 3 }
        };

        Context.Subjects.AddRange(subjects);

        var scheduledClasses = new List<ScheduledClass>
        {
            new() { SubjectId = subjects[0].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
            new() { SubjectId = subjects[1].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
            new() { SubjectId = subjects[2].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };

        Context.ScheduledClasses.AddRange(scheduledClasses);
        await Context.SaveChangesAsync();

        var expectedDtos = scheduledClasses.Select(sc => new ScheduledClassDto
        {
            Id = sc.Id,
            Subject = new SubjectDto
            {
                Id = subjects.Single(s => s.Id == sc.SubjectId).Id,
                Name = subjects.Single(s => s.Id == sc.SubjectId).Name,
                FacultyType = subjects.Single(s => s.Id == sc.SubjectId).FacultyType
            },
            StartTime = sc.StartTime,
            EndTime = sc.EndTime,
            Duration = sc.EndTime - sc.StartTime
        });

        // Act
        var dtos = await scheduledClassRepository.GetAllAsync<ScheduledClassDto>();

        // Assert
        dtos.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetAllScheduledClasses_WithPredicate_ShouldReturnDtos()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subjects = new List<Subject>
        {
            new() { Name = "TestSubject1", FacultyType = 1 },
            new() { Name = "TestSubject2", FacultyType = 2 },
            new() { Name = "TestSubject3", FacultyType = 3 }
        };

        Context.Subjects.AddRange(subjects);

        var scheduledClasses = new List<ScheduledClass>
        {
            new() { SubjectId = subjects[0].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
            new() { SubjectId = subjects[1].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
            new() { SubjectId = subjects[2].Id, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
        };

        Context.ScheduledClasses.AddRange(scheduledClasses);
        await Context.SaveChangesAsync();

        var expectedDtos = scheduledClasses
            .Where(s => s.Id != scheduledClasses[2].Id)
            .Select(sc => new ScheduledClassDto
                {
                    Id = sc.Id,
                    Subject = new SubjectDto
                    {
                        Id = subjects.Single(s => s.Id == sc.SubjectId).Id,
                        Name = subjects.Single(s => s.Id == sc.SubjectId).Name,
                        FacultyType = subjects.Single(s => s.Id == sc.SubjectId).FacultyType
                    },
                    StartTime = sc.StartTime,
                    EndTime = sc.EndTime,
                    Duration = sc.EndTime - sc.StartTime
                }
            );

        // Act
        var dtos = await scheduledClassRepository
            .GetAllAsync<ScheduledClassDto>(s => s.Id != scheduledClasses[2].Id);

        // Assert
        dtos.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetByIdSubject_ShouldReturnDto_IdExists()
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
    public async Task GetByIdScheduledClass_ShouldReturnDto_IdExists()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);

        var scheduledClass = new ScheduledClass
        {
            SubjectId = subject.Id,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };

        Context.ScheduledClasses.Add(scheduledClass);
        await Context.SaveChangesAsync();

        var expectedDto = new ScheduledClassDto
        {
            Id = scheduledClass.Id,
            Subject = new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                FacultyType = subject.FacultyType
            },
            StartTime = scheduledClass.StartTime,
            EndTime = scheduledClass.EndTime,
            Duration = scheduledClass.EndTime - scheduledClass.StartTime
        };

        // Act
        var dto = await scheduledClassRepository.GetByIdAsync<ScheduledClassDto>(scheduledClass.Id);

        // Assert
        dto.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetByIdScheduledClass_ShouldReturnNull_IdDoesNotExist()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        // Act
        var dto = await scheduledClassRepository.GetByIdAsync<ScheduledClassDto>(1);

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
    public async Task GetByIdScheduledClass_ShouldReturnEntity_IfExists()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);

        var scheduledClass = new ScheduledClass
        {
            SubjectId = subject.Id,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };

        Context.ScheduledClasses.Add(scheduledClass);
        await Context.SaveChangesAsync();

        // Act
        var entity = await scheduledClassRepository.GetByIdAsync(scheduledClass.Id);

        // Assert
        entity.Should().BeEquivalentTo(scheduledClass);
    }

    [Fact]
    public async Task GetByIdScheduledClass_ShouldReturnNull_IfDoesNotExist()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        // Act
        var entity = await scheduledClassRepository.GetByIdAsync(1);

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
    public async Task AddScheduledClass_ShouldAddEntity()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);

        var scheduledClass = new ScheduledClass
        {
            SubjectId = subject.Id,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };

        // Act
        scheduledClassRepository.Add(scheduledClass);
        await Context.SaveChangesAsync();

        // Assert
        Context.ScheduledClasses.Should().Contain(scheduledClass);
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

    [Fact]
    public async Task DeleteScheduledClass_ShouldDeleteEntity()
    {
        // Arrange
        var scheduledClassRepository = new CrudRepository<ScheduledClass>(Context, Mapper);

        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);

        var scheduledClass = new ScheduledClass
        {
            SubjectId = subject.Id,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };

        Context.ScheduledClasses.Add(scheduledClass);
        await Context.SaveChangesAsync();

        // Act
        scheduledClassRepository.Delete(scheduledClass);
        await Context.SaveChangesAsync();

        // Assert
        Context.ScheduledClasses.Should().NotContain(scheduledClass);
    }
}