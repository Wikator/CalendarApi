using AutoMapper;
using CalendarApp.Api.Configuration;
using CalendarApp.DataAccess.Repository;
using CalendarApp.Models.Entities;
using CalendarApp.Tests.RepositoryTests.Base;
using FluentAssertions;

namespace CalendarApp.Tests.RepositoryTests;

public class UnitOfWorkTests : RepositoryTestsBase
{
    private UnitOfWork UnitOfWork { get; }

    public UnitOfWorkTests()
    {
        UnitOfWork = new UnitOfWork(Context, InitializeMapper());
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnTrue_IfChangesWereMade()
    {
        // Arrange
        var subject = new Subject
        {
            Name = "TestSubject",
            FacultyType = 1
        };

        Context.Subjects.Add(subject);

        // Act
        var result = await UnitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnFalse_IfNoChangesWereMade()
    {
        // Act
        var result = await UnitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UserRepository_ShouldNotBeNull()
    {
        // Assert
        UnitOfWork.UserRepository.Should().NotBeNull();
    }

    [Fact]
    public void SubjectRepository_ShouldNotBeNull()
    {
        // Assert
        UnitOfWork.SubjectRepository.Should().NotBeNull();
    }

    [Fact]
    public void ScheduledClassRepository_ShouldNotBeNull()
    {
        // Assert
        UnitOfWork.ScheduledClassRepository.Should().NotBeNull();
    }
}