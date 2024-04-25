using System.Security.Claims;
using AutoMapper;
using CalendarApp.Api.Endpoints;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CalendarApp.Tests.EndpointTests;

public class ScheduledClassEndpointsTests
{
    private DefaultHttpContext HttpContext { get; }
    private Mock<IClaimsProvider> ClaimsProvider { get; }
    
    public ScheduledClassEndpointsTests()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "1"),
        }));
        
        HttpContext = new DefaultHttpContext { User = user };
        
        ClaimsProvider = new Mock<IClaimsProvider>();
        ClaimsProvider.Setup(x => x.GetUserIdOrDefault(HttpContext.User)).Returns(1);
        ClaimsProvider.Setup(x => x.GetUserId(HttpContext.User)).Returns(1);
    }
    
    // [Fact]
    // public async Task GetAll_ShouldReturnOk()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var scheduledClassDtos = new List<ScheduledClassDto>
    //     {
    //         SampleScheduledClassDto()
    //     };
    //     
    //     unitOfWork.Setup(x => x.ScheduledClassRepository.GetAllAsync<ScheduledClassDto>(1,
    //             s => s.StartTime >= DateTime.MinValue && s.StartTime <= DateTime.MaxValue))
    //         .ReturnsAsync(scheduledClassDtos);
    //
    //     // Act
    //     var get = await ScheduledClassEndpoints.GetAll(null, null,
    //         unitOfWork.Object, HttpContext, ClaimsProvider.Object);
    //
    //     // Assert
    //     var okResult = Assert.IsType<Ok<IEnumerable<ScheduledClassDto>>>(get);
    //     okResult.Value.Should().BeEquivalentTo(scheduledClassDtos);
    // }

    [Fact]
    public async Task GetById_ShouldReturnOk_IfScheduledClassExists()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var scheduledClassDto = SampleScheduledClassDto();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync<ScheduledClassDto>(1, 1))
            .ReturnsAsync(scheduledClassDto);
    
        // Act
        var get = await ScheduledClassEndpoints.GetById(unitOfWork.Object, HttpContext,
            ClaimsProvider.Object, 1);
    
        // Assert
        var okResult = Assert.IsType<Ok<ScheduledClassDto>>(get.Result);
        okResult.Value.Should().BeEquivalentTo(scheduledClassDto);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnNotFound_IfScheduledClassDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync<ScheduledClassDto>(1, 1))
            .ReturnsAsync((ScheduledClassDto?)null);
    
        // Act
        var get = await ScheduledClassEndpoints.GetById(unitOfWork.Object, HttpContext,
            ClaimsProvider.Object, 1);
    
        // Assert
        Assert.IsType<NotFound>(get.Result);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClassDto = SampleScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.Add(It.IsAny<ScheduledClass>()));
        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(It.IsAny<int>()))
            .ReturnsAsync(new SubjectDto
            {
                FacultyType = 0,
                Name = "Test",
                Id = 2
            });
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map<ScheduledClass>(upsertScheduledClassDto)).Returns(scheduledClass);
        mapper.Setup(x => x.Map<ScheduledClassDto>(scheduledClass)).Returns(scheduledClassDto);
    
        // Act
        var create = await ScheduledClassEndpoints.Create(unitOfWork.Object, upsertScheduledClassDto, mapper.Object);
    
        // Assert
        var result = Assert.IsType<Created<ScheduledClassDto>>(create.Result);
        result.Value.Should().BeEquivalentTo(scheduledClassDto);
    }
    
    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.Add(It.IsAny<ScheduledClass>()));
        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(It.IsAny<int>()));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map<ScheduledClass>(upsertScheduledClassDto)).Returns(scheduledClass);
    
        // Act
        var create = await ScheduledClassEndpoints.Create(unitOfWork.Object, upsertScheduledClassDto, mapper.Object);
    
        // Assert
        Assert.IsType<BadRequest<string>>(create.Result);
    }
    
    [Fact]
    public async Task Create_WithWrongSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(It.IsAny<int>())).
            ReturnsAsync((SubjectDto?)null);
        
        mapper.Setup(x => x.Map<ScheduledClass>(upsertScheduledClassDto)).Returns(scheduledClass);
    
        // Act
        var create = await ScheduledClassEndpoints.Create(unitOfWork.Object, upsertScheduledClassDto, mapper.Object);
    
        // Assert
        Assert.IsType<BadRequest<string>>(create.Result);
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_IfScheduledClassIsUpdated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
        var scheduledClassDto = SampleScheduledClassDto();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1, 1)).ReturnsAsync(scheduledClass);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map(upsertScheduledClassDto, scheduledClass));
        mapper.Setup(x => x.Map<ScheduledClassDto>(scheduledClass)).Returns(scheduledClassDto);
    
        // Act
        var update = await ScheduledClassEndpoints.Update(1, unitOfWork.Object, upsertScheduledClassDto, mapper.Object,
            HttpContext, ClaimsProvider.Object);
    
        // Assert
        var result = Assert.IsType<Ok<ScheduledClassDto>>(update.Result);
        result.Value.Should().BeEquivalentTo(scheduledClassDto);
    }
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_IfInvalidSubjectId()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1, 1)).ReturnsAsync(scheduledClass);

        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync<SubjectDto>(It.IsAny<int>()))
            .ReturnsAsync((SubjectDto?)null);
        
        mapper.Setup(x => x.Map(upsertScheduledClassDto, scheduledClass));
    
        // Act
        var update = await ScheduledClassEndpoints.Update(1, unitOfWork.Object, upsertScheduledClassDto, mapper.Object,
            HttpContext, ClaimsProvider.Object);
    
        // Assert
        Assert.IsType<BadRequest<string>>(update.Result);
    }
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_IfScheduledClassIsNotUpdated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1, 1)).ReturnsAsync(scheduledClass);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map(upsertScheduledClassDto, scheduledClass));
    
        // Act
        var update = await ScheduledClassEndpoints.Update(1, unitOfWork.Object, upsertScheduledClassDto, mapper.Object,
            HttpContext, ClaimsProvider.Object);
    
        // Assert
        Assert.IsType<BadRequest<string>>(update.Result);
    }
    
    [Fact]
    public async Task Update_ShouldReturnNotFound_IfScheduledClassDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
    
        var upsertScheduledClassDto = SampleUpsertScheduledClassDto();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1, 1)).ReturnsAsync((ScheduledClass?)null);
    
        // Act
        var update = await ScheduledClassEndpoints.Update(1, unitOfWork.Object, upsertScheduledClassDto, mapper.Object,
            HttpContext, ClaimsProvider.Object);
    
        // Assert
        Assert.IsType<NotFound>(update.Result);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNoContent_IfScheduledClassIsDeleted()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1)).ReturnsAsync(scheduledClass);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
    
        // Act
        var delete = await ScheduledClassEndpoints.Delete(1, unitOfWork.Object);
    
        // Assert
        Assert.IsType<NoContent>(delete.Result);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_IfScheduledClassDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1)).ReturnsAsync((ScheduledClass?)null);
    
        // Act
        var delete = await ScheduledClassEndpoints.Delete(1, unitOfWork.Object);
    
        // Assert
        Assert.IsType<NotFound>(delete.Result);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnBadRequest_IfScheduledClassIsNotDeleted()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var scheduledClass = SampleScheduledClass();
    
        unitOfWork.Setup(x => x.ScheduledClassRepository.GetByIdAsync(1)).ReturnsAsync(scheduledClass);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
    
        // Act
        var delete = await ScheduledClassEndpoints.Delete(1, unitOfWork.Object);
    
        // Assert
        Assert.IsType<BadRequest<string>>(delete.Result);
    }

    private static ScheduledClassDto SampleScheduledClassDto()
    {
        return new ScheduledClassDto
        {
            Id = 1,
            Duration = new TimeSpan(0, 1, 30, 0),
            StartTime = new DateTime(),
            EndTime = DateTime.Now.AddMinutes(90),
            Subject = new SubjectDto
            {
                Id = 1,
                Name = "TestSubject1",
                FacultyType = 2
            }
        };
    }

    private static ScheduledClass SampleScheduledClass()
    {
        return new ScheduledClass
        {
            Id = 1,
            StartTime = new DateTime(),
            EndTime = DateTime.Now.AddMinutes(90),
            SubjectId = 1
        };
    }

    private static UpsertScheduledClassDto SampleUpsertScheduledClassDto()
    {
        return new UpsertScheduledClassDto
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddMinutes(90),
            SubjectId = 1
        };
    }
}