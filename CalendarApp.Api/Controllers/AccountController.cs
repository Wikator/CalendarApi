using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AccountController(
    IUnitOfWork unitOfWork,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto, IMapper mapper)
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

        unitOfWork.UserRepository.Register(user);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to register user."));

        var userDto = mapper.Map<UserDto>(user);
        var userWithTokenDto = userDto.ToUserWithTokenDto(tokenService.CreateToken(userDto));
        return Ok(userWithTokenDto);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var userDto = await unitOfWork.UserRepository.LoginAsync(loginDto.Username, loginDto.Password);

        if (userDto is null)
            return BadRequest(new ErrorMessage("Invalid username or password."));

        var userWithTokenDto = userDto.ToUserWithTokenDto(tokenService.CreateToken(userDto));
        return Ok(userWithTokenDto);
    }
}
