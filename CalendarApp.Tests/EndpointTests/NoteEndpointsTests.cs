using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CalendarApp.Api.Endpoints;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace CalendarApp.Tests.EndpointTests;

public class NoteEndpointsTests
{
    private DefaultHttpContext HttpContext { get; }
    private Mock<IClaimsProvider> ClaimsProvider { get; }
    
    public NoteEndpointsTests()
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
    
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var notes = new List<NoteDto>
        {
            new() { Id = 1, Content = "Note 1", Title = "Title 1", LastModified = DateTime.Now },
            new() { Id = 2, Content = "Note 2", Title = "Title 2", LastModified = DateTime.Now }
        };
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetAllAsync<NoteDto>(1, It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(notes);
        
        // Act
        var result = await NoteEndpoints.GetAll(unitOfWork.Object, HttpContext, ClaimsProvider.Object, 1);
        
        // Assert
        Assert.IsType<Ok<IEnumerable<NoteDto>>>(result);
        Assert.Equal(notes, result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_IfNoteExists()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        
        var note = new NoteDto { Id = 1, Content = "Note 1", Title = "Title 1", LastModified = DateTime.Now };
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync<NoteDto>(1, 1)).ReturnsAsync(note);
        
        // Act
        var get = await NoteEndpoints.GetById(unitOfWork.Object, HttpContext, ClaimsProvider.Object, 1);
        
        // Assert
        var result = Assert.IsType<Ok<NoteDto>>(get.Result);
        Assert.Equal(note, result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_IfNoteDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync<NoteDto>(1, 1)).ReturnsAsync((NoteDto?)null);
        
        // Act
        var get = await NoteEndpoints.GetById(unitOfWork.Object, HttpContext, ClaimsProvider.Object, 1);
        
        // Assert
        Assert.IsType<NotFound>(get.Result);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Subject
            {
                FacultyType = 0,
                Id = 1,
                Name = "Test",
                ScheduledClasses = new List<ScheduledClass>()
            });
        noteRepository.Setup(x => x.Add(It.IsAny<Note>(), It.IsAny<int>()));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map<Note>(upsertNoteDto)).Returns(note);
        mapper.Setup(x => x.Map<NoteDto>(note)).Returns(new NoteDto { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now });
        
        // Act
        var create = await NoteEndpoints.Create(unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object, 1);
        
        // Assert
        var result = Assert.IsType<Created<NoteDto>>(create.Result);
        Assert.Equal("api/notes/1", result.Location);
        Assert.Equal(note.Id, result.Value?.Id);
    }
    
    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Subject
            {
                FacultyType = 0,
                Id = 1,
                Name = "Test",
                ScheduledClasses = new List<ScheduledClass>()
            });
        noteRepository.Setup(x => x.Add(It.IsAny<Note>(), It.IsAny<int>()));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map<Note>(upsertNoteDto)).Returns(note);
        
        // Act
        var create = await NoteEndpoints.Create(unitOfWork.Object,
            upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object, 1);
        
        // Assert
        Assert.IsType<BadRequest<string>>(create.Result);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsOk()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map(upsertNoteDto, note));
        mapper.Setup(x => x.Map<NoteDto>(note)).Returns(new NoteDto { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now });
        
        // Act
        var update = await NoteEndpoints.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        var result = Assert.IsType<Ok<NoteDto>>(update.Result);
        Assert.Equal(note.Id, result.Value?.Id);
    }
    
    [Fact]
    public async Task Update_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map(upsertNoteDto, note));
        
        // Act
        var update = await NoteEndpoints.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        Assert.IsType<BadRequest<string>>(update.Result);
    }
    
    [Fact]
    public async Task Update_ReturnsNotFound_IfNoteDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync((Note?)null);
        
        // Act
        var update = await NoteEndpoints.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        Assert.IsType<NotFound>(update.Result);
    }
    
    [Fact]
    public async Task Delete_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        
        // Act
        var delete = await NoteEndpoints.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        Assert.IsType<NoContent>(delete.Result);
    }
    
    [Fact]
    public async Task Delete_ReturnsBadRequest_IfNoteFailsToDelete()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        
        // Act
        var delete = await NoteEndpoints.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        Assert.IsType<BadRequest<string>>(delete.Result);
    }
    
    [Fact]
    public async Task Delete_ReturnsNotFound_IfNoteDoesNotExist()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
        
        unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
        noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync((Note?)null);
        
        // Act
        var delete = await NoteEndpoints.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
        
        // Assert
        Assert.IsType<NotFound>(delete.Result);
    }
}