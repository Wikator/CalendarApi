using System.Security.Cryptography;
using AutoMapper;
using CalendarApp.Api.Configuration;
using CalendarApp.DataAccess.Repository;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Dtos.Responses.Subject;
using CalendarApp.Models.Dtos.Responses.User;
using CalendarApp.Models.Entities;
using CalendarApp.Tests.RepositoryTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Tests.RepositoryTests;

public class UserRepositoryTests : RepositoryTestsBase
{
    private UserRepository UserRepository { get; }
    private Subject Subject { get; }
    
    public UserRepositoryTests()
    {
        Subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(Subject);
        Context.SaveChanges();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<AutoMapperProfiles>());

        var mapper = mapperConfig.CreateMapper();

        UserRepository = new UserRepository(Context, mapper);
    }

    [Fact]
    public async Task Register_ShouldCreateUser()
    {
        // Arrange
        var user = SampleUser(Subject.Id);

        var expectedUser = new User
        {
            Id = 1,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            Faculty1Id = user.Faculty1Id,
            Faculty1 = Subject,
            Faculty2Id = null,
            Faculty3Id = null,
            Role = "User"
        };

        // Act
        UserRepository.Register(user);
        await Context.SaveChangesAsync();

        // Assert
        var createdUser = await Context.Users.Include(u => u.Faculty1).SingleAsync();
        createdUser.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUserDto_IfPasswordIsCorrect()
    {
        // Arrange
        using HMACSHA512 hmac = new();
        var passwordHash = hmac.ComputeHash("test"u8.ToArray());
        var passwordSalt = hmac.Key;

        var user = new User
        {
            Username = "testuser",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Faculty1Id = Subject.Id
        };

        var expectedUserDto = new UserDto
        {
            Id = 1,
            Username = user.Username,
            Faculty1 = new SubjectDto
            {
                Id = Subject.Id,
                Name = Subject.Name,
                FacultyType = Subject.FacultyType
            },
            Role = "User"
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        await Context.SaveChangesAsync();

        // Act
        var userDto = await UserRepository.LoginAsync(user.Username, "test");

        // Assert
        userDto.Should().NotBeNull();
        userDto!.Should().BeEquivalentTo(expectedUserDto);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_IfPasswordIsIncorrect()
    {
        // Arrange
        using HMACSHA512 hmac = new();
        var passwordHash = hmac.ComputeHash("test"u8.ToArray());
        var passwordSalt = hmac.Key;

        var user = new User
        {
            Username = "testuser",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Faculty1Id = Subject.Id
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var userDto = await UserRepository.LoginAsync(user.Username, "wrongpassword");

        // Assert
        userDto.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_IfUserDoesNotExist()
    {
        // Arrange
        // Act
        var userDto = await UserRepository.LoginAsync("nonexistentuser", "password");

        // Assert
        userDto.Should().BeNull();
    }

    private static User SampleUser(int? subjectId)
    {
        return new User
        {
            Username = "testuser",
            PasswordHash = [0, 1, 0],
            PasswordSalt = [0, 1, 0],
            Faculty1Id = subjectId
        };
    }
}