using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using CalendarApp.Api.Controllers;
using CalendarApp.Api.Services;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CalendarApp.Tests.ControllerTests;

public class NotesControllerTests
{
    private Mock<IUnitOfWork> UnitOfWorkMock { get; }
    private NotesController NotesController { get; }
    private const int UserId = 1;
    
    public NotesControllerTests()
    {
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        
        var claimsProviderMock = new Mock<IClaimsProvider>();
        claimsProviderMock.Setup(x => x.GetUserIdOrDefault(It.IsAny<ClaimsPrincipal>()))
            .Returns(UserId);
        
        claimsProviderMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(UserId);

        NotesController = new NotesController(UnitOfWorkMock.Object, claimsProviderMock.Object);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var notes = new List<NoteDto>
        {
            new() { Id = 1, Content = "Note 1", Title = "Title 1", LastModified = DateTime.Now },
            new() { Id = 2, Content = "Note 2", Title = "Title 2", LastModified = DateTime.Now }
        };
        UnitOfWorkMock.Setup(x => x.NoteRepository.GetAllAsync<NoteDto>(UserId,
                It.IsAny<Expression<Func<Note, bool>>>())).ReturnsAsync(notes);
        
        // Act
        var allNotes = await NotesController.GetAll(UserId);
        
        // Assert
        var result = Assert.IsType<OkObjectResult>(allNotes);
        Assert.Equal(notes, result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_IfNoteExists()
    {
        // Arrange
        var note = SampleNoteDto();
        UnitOfWorkMock.Setup(x => x.NoteRepository.GetByIdAsync<NoteDto>(UserId, note.Id))
            .ReturnsAsync(note);
        
        // Act
        var get = await NotesController.GetById(note.Id);
        
        // Assert
        var result = Assert.IsType<OkObjectResult>(get);
        Assert.Equal(note, result.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_IfNoteDoesNotExist()
    {
        // Arrange
        const int wrongNoteId = 1;
        UnitOfWorkMock.Setup(x => x.NoteRepository.GetByIdAsync<NoteDto>(UserId, wrongNoteId))
            .ReturnsAsync((NoteDto?)null);
        
        // Act
        var get = await NotesController.GetById(wrongNoteId);
        
        // Assert
        Assert.IsType<NotFoundResult>(get);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        const int scheduledClassId = 1;
        var mapper = new Mock<IMapper>();
        var upsertNoteDto = SampleUpsertNoteDto();
        var note = SampleNote();
        
        UnitOfWorkMock.Setup(x => x.ScheduledClassRepository.ExistsAsync(x => x.Id == scheduledClassId))
            .ReturnsAsync(true);
        UnitOfWorkMock.Setup(x => x.NoteRepository.Add(It.IsAny<Note>(), UserId));
        UnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        mapper.Setup(x => x.Map<Note>(upsertNoteDto)).Returns(note);
        mapper.Setup(x => x.Map<NoteDto>(note)).Returns(SampleNoteDto());
        
        // Act
        var create = await NotesController.Create(scheduledClassId, upsertNoteDto, mapper.Object);
        
        // Assert
        var result = Assert.IsType<CreatedAtActionResult>(create);
    }
    
    // TODO: Finish refactor
    
    // [Fact]
    // public async Task Create_WithInvalidData_ReturnsBadRequest()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var mapper = new Mock<IMapper>();
    //     var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
    //     var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     unitOfWork.Setup(x => x.SubjectRepository.GetByIdAsync(It.IsAny<int>()))
    //         .ReturnsAsync(new Subject
    //         {
    //             FacultyType = 0,
    //             Id = 1,
    //             Name = "Test",
    //             ScheduledClasses = new List<ScheduledClass>()
    //         });
    //     noteRepository.Setup(x => x.Add(It.IsAny<Note>(), It.IsAny<int>()));
    //     unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
    //     mapper.Setup(x => x.Map<Note>(upsertNoteDto)).Returns(note);
    //     
    //     // Act
    //     var create = await NotesController.Create(unitOfWork.Object,
    //         upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object, 1);
    //     
    //     // Assert
    //     Assert.IsType<BadRequest<string>>(create.Result);
    // }
    //
    // [Fact]
    // public async Task Update_WithValidData_ReturnsOk()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var mapper = new Mock<IMapper>();
    //     var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
    //     var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
    //     unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
    //     mapper.Setup(x => x.Map(upsertNoteDto, note));
    //     mapper.Setup(x => x.Map<NoteDto>(note)).Returns(new NoteDto { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now });
    //     
    //     // Act
    //     var update = await NotesController.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     var result = Assert.IsType<Ok<NoteDto>>(update.Result);
    //     Assert.Equal(note.Id, result.Value?.Id);
    // }
    //
    // [Fact]
    // public async Task Update_WithInvalidData_ReturnsBadRequest()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var mapper = new Mock<IMapper>();
    //     var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
    //     var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
    //     unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
    //     mapper.Setup(x => x.Map(upsertNoteDto, note));
    //     
    //     // Act
    //     var update = await NotesController.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     Assert.IsType<BadRequest<string>>(update.Result);
    // }
    //
    // [Fact]
    // public async Task Update_ReturnsNotFound_IfNoteDoesNotExist()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var mapper = new Mock<IMapper>();
    //     var upsertNoteDto = new UpsertNoteDto { Title = "Title", Content = "Content" };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync((Note?)null);
    //     
    //     // Act
    //     var update = await NotesController.Update(1, unitOfWork.Object, upsertNoteDto, mapper.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     Assert.IsType<NotFound>(update.Result);
    // }
    //
    // [Fact]
    // public async Task Delete_WithValidData_ReturnsNoContent()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
    //     unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
    //     
    //     // Act
    //     var delete = await NotesController.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     Assert.IsType<NoContent>(delete.Result);
    // }
    //
    // [Fact]
    // public async Task Delete_ReturnsBadRequest_IfNoteFailsToDelete()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     var note = new Note { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(note);
    //     unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
    //     
    //     // Act
    //     var delete = await NotesController.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     Assert.IsType<BadRequest<string>>(delete.Result);
    // }
    //
    // [Fact]
    // public async Task Delete_ReturnsNotFound_IfNoteDoesNotExist()
    // {
    //     // Arrange
    //     var unitOfWork = new Mock<IUnitOfWork>();
    //     var noteRepository = new Mock<IAuthorizedCrudRepository<Note>>();
    //     
    //     unitOfWork.Setup(x => x.NoteRepository).Returns(noteRepository.Object);
    //     noteRepository.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync((Note?)null);
    //     
    //     // Act
    //     var delete = await NotesController.Delete(1, unitOfWork.Object, HttpContext, ClaimsProvider.Object);
    //     
    //     // Assert
    //     Assert.IsType<NotFound>(delete.Result);
    // }

    private static UpsertNoteDto SampleUpsertNoteDto() => new() { Title = "Title", Content = "Content" };
    private static Note SampleNote() =>
        new() { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };

    private static NoteDto SampleNoteDto() =>
        new() { Id = 1, Title = "Title", Content = "Content", LastModified = DateTime.Now };
}