using AutoMapper;
using CalendarApp.Api.Endpoints;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CalendarApp.Tests.EndpointTests;

public class SubjectEndpointsTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var subjectDtos = new List<SubjectDto>
        {
            new() { Id = 1, Name = "TestSubject1" },
            new() { Id = 2, Name = "TestSubject2" }
        };

        unitOfWork.Setup(x => x.SubjectRepository.GetAllAsync<SubjectDto>(null)).ReturnsAsync(subjectDtos);

        // Act
        var get = await SubjectEndpoints.GetAll(unitOfWork.Object);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<SubjectDto>>>(get);
        okResult.Value.Should().BeEquivalentTo(subjectDtos);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_IfSubjectExists()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var subjectDto = new SubjectDto { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(1)).ReturnsAsync(subjectDto);

        // Act
        var get = await SubjectEndpoints.GetById(unitOfWork.Object, 1);

        // Assert
        var okResult = Assert.IsType<Ok<SubjectDto>>(get.Result);
        okResult.Value.Should().BeEquivalentTo(subjectDto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_IfSubjectDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(1)).ReturnsAsync((SubjectDto?)null);

        // Act
        var get = await SubjectEndpoints.GetById(unitOfWork.Object, 1);

        // Assert
        Assert.IsType<NotFound>(get.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_IfSubjectIsCreated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var upsertSubjectDto = new UpsertSubjectDto { Name = "TestSubject" };
        var subject = new Subject { Name = "TestSubject" };
        var subjectDto = new SubjectDto { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.Add(subject));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map<Subject>(upsertSubjectDto)).Returns(subject);
        mapper.Setup(x => x.Map<SubjectDto>(subject)).Returns(subjectDto);

        // Act
        var post = await SubjectEndpoints.Create(unitOfWork.Object, upsertSubjectDto, mapper.Object);

        // Assert
        var createdResult = Assert.IsType<Created<SubjectDto>>(post.Result);
        createdResult.Location.Should().Be("api/subjects/1");
        createdResult.Value.Should().BeEquivalentTo(subjectDto);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_IfSubjectIsNotCreated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var upsertSubjectDto = new UpsertSubjectDto { Name = "TestSubject" };
        var subject = new Subject { Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.Add(subject));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map<Subject>(upsertSubjectDto)).Returns(subject);

        // Act
        var post = await SubjectEndpoints.Create(unitOfWork.Object, upsertSubjectDto, mapper.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(post.Result);
        badRequestResult.Value.Should().Be("Failed to create subject.");
    }

    [Fact]
    public async Task Update_ShouldReturnOk_IfSubjectIsUpdated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var upsertSubjectDto = new UpsertSubjectDto { Name = "TestSubject" };
        var subject = new Subject { Id = 1, Name = "TestSubject" };
        var subjectDto = new SubjectDto { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync(subject);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map(upsertSubjectDto, subject));
        mapper.Setup(x => x.Map<SubjectDto>(subject)).Returns(subjectDto);

        // Act
        var put = await SubjectEndpoints.Update(1, unitOfWork.Object, upsertSubjectDto, mapper.Object);

        // Assert
        var okResult = Assert.IsType<Ok<SubjectDto>>(put.Result);
        okResult.Value.Should().BeEquivalentTo(subjectDto);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_IfSubjectIsNotUpdated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var upsertSubjectDto = new UpsertSubjectDto { Name = "TestSubject" };
        var subject = new Subject { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync(subject);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map(upsertSubjectDto, subject));

        // Act
        var put = await SubjectEndpoints.Update(1, unitOfWork.Object, upsertSubjectDto, mapper.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(put.Result);
        badRequestResult.Value.Should().Be("Failed to update subject.");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_IfSubjectDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var upsertSubjectDto = new UpsertSubjectDto { Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync((Subject?)null);

        // Act
        var put = await SubjectEndpoints.Update(1, unitOfWork.Object, upsertSubjectDto, mapper.Object);

        // Assert
        Assert.IsType<NotFound>(put.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_IfSubjectIsDeleted()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        var subject = new Subject { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync(subject);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var delete = await SubjectEndpoints.Delete(1, unitOfWork.Object);

        // Assert
        Assert.IsType<NoContent>(delete.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_IfSubjectIsNotDeleted()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        var subject = new Subject { Id = 1, Name = "TestSubject" };

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync(subject);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

        // Act
        var delete = await SubjectEndpoints.Delete(1, unitOfWork.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(delete.Result);
        badRequestResult.Value.Should().Be("Failed to delete subject.");
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_IfSubjectDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(1)).ReturnsAsync((Subject?)null);

        // Act
        var delete = await SubjectEndpoints.Delete(1, unitOfWork.Object);

        // Assert
        Assert.IsType<NotFound>(delete.Result);
    }
}