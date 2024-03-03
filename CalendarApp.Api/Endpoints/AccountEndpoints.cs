using System.Security.Cryptography;
using System.Text;
using CalendarApp.Api.Data.Repository.Contracts;
using CalendarApp.Api.Dtos.Requests;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;
using CalendarApp.Api.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var accountApi = app.MapGroup("api/account");

        accountApi.MapPost("register", Register);
        accountApi.MapPost("login", Login);
    }

    public static async Task<Results<Ok<UserWithTokenDto>, BadRequest<string>>> Register(IUnitOfWork unitOfWork,
        RegisterDto registerDto, ITokenService tokenService)
    {
        using HMACSHA512 hmac = new();
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        var passwordSalt = hmac.Key;

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = registerDto.Username == "Admin" ? "Admin" : "User"
        };

        var userDto = unitOfWork.UserRepository.Register(user);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to register user.");

        var userWithTokenDto = userDto.ToUserWithTokenDto(tokenService.CreateToken(userDto));
        return TypedResults.Ok(userWithTokenDto);
    }

    public static async Task<Results<Ok<UserWithTokenDto>, BadRequest<string>>> Login(IUnitOfWork unitOfWork,
        LoginDto loginDto, ITokenService tokenService)
    {
        var userDto = await unitOfWork.UserRepository.LoginAsync(loginDto.Username, loginDto.Password);

        if (userDto is null)
            return TypedResults.BadRequest("Invalid username or password.");

        var userWithTokenDto = userDto.ToUserWithTokenDto(tokenService.CreateToken(userDto));
        return TypedResults.Ok(userWithTokenDto);
    }
}