using AutoMapper;
using CalendarApp.DataAccess.Repository;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using CalendarApp.Tests.RepositoryTests.Base;
using FluentAssertions;

namespace CalendarApp.Tests.RepositoryTests;

public class ScheduledClassRepositoryTests : RepositoryTestsBase
{
    private IMapper Mapper { get; } = InitializeMapper();
    private ScheduledClassRepository Repository { get; }
    
    public ScheduledClassRepositoryTests()
    {
        Repository = new ScheduledClassRepository(Context, Mapper);
    }

    [Fact]
    public async Task GetAllScheduledClasses_ShouldReturnSubjectsWithNoNotes_WhenNotLoggedIn()
    {
        // Arrange
        await SeedSubjectWithNotes();
        
        // Act
        var result = (await Repository.GetAllAsync<ScheduledClassDto>(null)).ToList();
        
        // Assert
        result.Should().HaveCount(1);
        result.Single().Notes.Should().HaveCount(0);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    public async Task GetAllScheduledClasses_ShouldReturnSubjectsWithUserNotes_WhenLoggedIn(int userId,
        int expectedCount)
    {
        // Arranged
        await SeedSubjectWithNotes();
        
        var user = new User
        {
            Id = userId,
            Username = "Test",
            PasswordHash = [],
            PasswordSalt = [],
            Faculty1Id = 1
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = (await Repository.GetAllAsync<ScheduledClassDto>(userId)).ToList();
        
        // Assert
        result.Should().HaveCount(1);
        result.Single().Notes.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GetAllSubjects_ShouldNotReturnSubjects_WithPredicate()
    {
        // Arrange
        await SeedSubjectWithNotes();
        
        // Act
        var result = await Repository.GetAllAsync<ScheduledClassDto>(null, s => s.IsCancelled);
        
        // Arrange
        result.Should().HaveCount(0);
    }
    
    [Fact]
    public async Task GetAllSubjects_ShouldReturnSubjects_WithPredicate()
    {
        // Arrange
        await SeedSubjectWithNotes();
        
        // Act
        var result = await Repository.GetAllAsync<ScheduledClassDto>(null, s => s.SubjectId == 1);
        
        // Arrange
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllSubjects_ShouldNotReturnSubjects_WhenLoggedInAndWrongFaculty()
    {
        // Arranged
        await SeedSubjectWithNotes();
        
        var user = new User
        {
            Username = "Test",
            PasswordHash = [],
            PasswordSalt = []
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        
        // Act
        var result = await Repository.GetAllAsync<ScheduledClassDto>(1);
        
        // Assert
        result.Should().HaveCount(0);
    }

    private async Task SeedSubjectWithNotes()
    {
        var notes = new List<Note>
        {
            new()
            {
                Title = "Test1",
                Content = "Test1",
                UserId = 1,
                ScheduledClassId = 1
            },
            new()
            {
                Title = "Test2",
                Content = "Test2",
                UserId = 2,
                ScheduledClassId = 1
            }
        };

        var subject = new Subject
        {
            Name = "Test",
            FacultyType = 1
        };

        var scheduledClass = new ScheduledClass
        {
            SubjectId = 1
        };

        Context.Notes.AddRange(notes);
        Context.Subjects.Add(subject);
        Context.ScheduledClasses.Add(scheduledClass);
        await Context.SaveChangesAsync();
    }
}