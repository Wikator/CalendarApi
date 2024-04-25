using AutoMapper;
using CalendarApp.DataAccess.Repository;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using CalendarApp.Tests.RepositoryTests.Base;
using FluentAssertions;

namespace CalendarApp.Tests.RepositoryTests;

public class AuthorizedCrudRepositoryTests : RepositoryTestsBase
{
    private IMapper Mapper { get; } = InitializeMapper();

    [Fact]
    public async Task GetAllNotes_ShouldReturnDtos()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var notes = new List<Note>
        {
            new()
            {
                Title = "Test1",
                Content = "Test1",
                UserId = 1
            },
            new()
            {
                Title = "Test2",
                Content = "Test2",
                UserId = 2
            }
        };
        Context.Notes.AddRange(notes);
        await Context.SaveChangesAsync();

        // Act
        var result = (await repository.GetAllAsync<NoteDto>(1)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Test1");
    }
    
    [Fact]
    public async Task GetAllNotes_WithPredicate_ShouldReturnDtos()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var notes = new List<Note>
        {
            new()
            {
                Title = "Test1",
                Content = "Test1",
                UserId = 1
            },
            new ()
            {
                Title = "No",
                Content = "No",
                UserId = 1
            },
            new()
            {
                Title = "Test2",
                Content = "Test2",
                UserId = 2
            }
        };
        await Context.Notes.AddRangeAsync(notes);
        await Context.SaveChangesAsync();

        // Act
        var result = (await repository.GetAllAsync<NoteDto>(1, n => n.Title != "No")).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Test1");
    }
    
    [Fact]
    public async Task GetNoteById_ShouldReturnDto_WhenIdExists()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync<NoteDto>(1, note.Id);

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetNoteById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync<NoteDto>(1, 2);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetNoteById_ShouldReturnNull_WhenUserIsNotOwner()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync<NoteDto>(2, note.Id);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetNoteById_ShouldReturnEntity_WhenIdExists()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1, note.Id);

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetNoteById_ShouldNotReturnEntity_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1, 2);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetNoteById_ShouldNotReturnEntity_WhenUserIsNotOwner()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(2, note.Id);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task AddNote_ShouldCreateEntity()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1,
        };

        // Act
        repository.Add(note, 1);
        await Context.SaveChangesAsync();

        // Assert
        Context.Notes.Should().Contain(note);
    }
    
    [Fact]
    public async Task DeleteNote_ShouldDeleteEntity()
    {
        // Arrange
        var repository = new AuthorizedCrudRepository<Note>(Context, Mapper);
        var note = new Note
        {
            Title = "Test1",
            Content = "Test1",
            UserId = 1
        };
        Context.Notes.Add(note);
        await Context.SaveChangesAsync();

        // Act
        await repository.DeleteByIdAsync(note.Id, 1);
        await Context.SaveChangesAsync();

        // Assert
        Context.Notes.Should().NotContain(note);
    }
}
