using AutoMapper;
using CalendarApp.Api.Endpoints;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CalendarApp.Tests.EndpointTests;

public class AccountEndpointsTests
{
    private Mock<ITokenService> TokenServiceMock { get; }
    private const string TestToken = "TestToken";

    public AccountEndpointsTests()
    {
        TokenServiceMock = new Mock<ITokenService>();
        TokenServiceMock.Setup(x => x.CreateToken(It.IsAny<UserDto>())).Returns(TestToken);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        var registerDto = new RegisterDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        unitOfWork.Setup(x => x.UserRepository.Register(It.IsAny<User>()));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(SampleUserDto());

        // Act
        var register = await AccountEndpoints.Register(unitOfWork.Object, registerDto, mapper.Object,
            TokenServiceMock.Object);

        // Assert
        var result = Assert.IsType<Ok<UserWithTokenDto>>(register.Result);
        result.Value.Should().BeEquivalentTo(new UserWithTokenDto
        {
            Username = registerDto.Username,
            Token = "TestToken",
            Role = "User"
        });
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        var registerDto = new RegisterDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        var mapper = new Mock<IMapper>();

        unitOfWork.Setup(x => x.UserRepository.Register(It.IsAny<User>()));
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);
        mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(SampleUserDto());

        // Act
        var register = await AccountEndpoints.Register(unitOfWork.Object, registerDto,
            mapper.Object, TokenServiceMock.Object);

        // Assert
        var result = Assert.IsType<BadRequest<string>>(register.Result);
        result.Value.Should().Be("Failed to register user.");
    }

    [Fact]
    public async Task Login_WithValidData_ReturnsOk()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();

        var loginDto = new LoginDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        var userDto = SampleUserDto();

        unitOfWork.Setup(x => x.UserRepository.LoginAsync(loginDto.Username, loginDto.Password)).ReturnsAsync(userDto);

        // Act
        var login = await AccountEndpoints.Login(unitOfWork.Object, loginDto, TokenServiceMock.Object);

        // Assert
        var result = Assert.IsType<Ok<UserWithTokenDto>>(login.Result);
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
        var unitOfWork = new Mock<IUnitOfWork>();

        var loginDto = new LoginDto
        {
            Username = "TestUser",
            Password = "TestPassword"
        };

        unitOfWork.Setup(x => x.UserRepository.LoginAsync(loginDto.Username, loginDto.Password))
            .ReturnsAsync((UserDto?)null);

        // Act
        var login = await AccountEndpoints.Login(unitOfWork.Object, loginDto, TokenServiceMock.Object);

        // Assert
        var result = Assert.IsType<BadRequest<string>>(login.Result);
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