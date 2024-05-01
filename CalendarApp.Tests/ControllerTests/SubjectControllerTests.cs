using CalendarApp.Api.Controllers;
using CalendarApp.DataAccess.Repository.Contracts;
using Moq;

namespace CalendarApp.Tests.ControllerTests;

public class SubjectControllerTests
{
    private Mock<IUnitOfWork> UnitOfWorkMock { get; } 
    private SubjectsController SubjectsController { get; }

    public SubjectControllerTests()
    {
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        SubjectsController = new SubjectsController(UnitOfWorkMock.Object);
    }
    
    // [Fact]
    // public async Task GetAll_ReturnsOkObjectResult()
    // {
    //     // Arrange
    //     var subjectDtos = new List<SubjectDto> { new SubjectDto() };
    //     UnitOfWorkMock.Setup(u => u.SubjectRepository.GetAllAsync<SubjectDto>())
    //         .ReturnsAsync(subjectDtos);
    //     
    //     // Act
    //     var result = await SubjectsController.GetAll();
    //     
    //     // Assert
    //     var okObjectResult = Assert.IsType<OkObjectResult>(result);
    //     var model = Assert.IsAssignableFrom<IEnumerable<SubjectDto>>(okObjectResult.Value);
    //     Assert.Single(model);
    // }
}