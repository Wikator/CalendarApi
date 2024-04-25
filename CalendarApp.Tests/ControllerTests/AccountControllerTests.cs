using AutoMapper;
using CalendarApp.Api.Controllers;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CalendarApp.Tests.ControllerTests;

public class AccountControllerTests
{
    private Mock<IUnitOfWork> UnitOfWorkMock { get; }
    private AccountController AccountController { get; }
    private const string TestToken = "TestToken";

    public AccountControllerTests()
    {
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<UserDto>())).Returns(TestToken);
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        AccountController = new AccountController(UnitOfWorkMock.Object, tokenServiceMock.Object);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        UnitOfWorkMock.Setup(x => x.UserRepository.Register(It.IsAny<User>()));
        UnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(SampleUserDto());

        // Act
        var register = await AccountController.Register(registerDto, mapper.Object);

        // Assert
        var result = Assert.IsType<OkObjectResult>(register);
        result.Value.Should().BeEquivalentTo(new UserWithTokenDto
        {
            Username = registerDto.Username,
            Token = "TestToken",
            Role = "User"
        });
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsUnprocessableEntity()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        var mapper = new Mock<IMapper>();

        UnitOfWorkMock.Setup(x => x.UserRepository.Register(It.IsAny<User>()));
        UnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(SampleUserDto());

        // Act
        var register = await AccountController.Register(registerDto, mapper.Object);

        // Assert
        var result = Assert.IsType<UnprocessableEntityObjectResult>(register);
        result.Value.Should().Be("Failed to register user.");
    }

    [Fact]
    public async Task Login_WithValidData_ReturnsOk()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        var userDto = SampleUserDto();

        UnitOfWorkMock.Setup(x => x.UserRepository.LoginAsync(loginDto.Username, loginDto.Password))
            .ReturnsAsync(userDto);
        
        // Act
        var login = await AccountController.Login(loginDto);

        // Assert
        var result = Assert.IsType<OkObjectResult>(login);
        result.Value.Should().BeEquivalentTo(new UserWithTokenDto
        {
            Username = loginDto.Username,
            Token = "TestToken",
            Role = "User"
        });
    }

    [Fact]
    public async Task Login_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        UnitOfWorkMock.Setup(x => x.UserRepository.LoginAsync(loginDto.Username, loginDto.Password))
            .ReturnsAsync((UserDto?)null);
        
        // Act
        var login = await AccountController.Login(loginDto);

        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(login);
        result.Value.Should().Be("Invalid username or password.");
    }

    private static UserDto SampleUserDto()
    {
        return new UserDto
        {
            Username = "TestUser",
            Role = "User"
        };
    }
}