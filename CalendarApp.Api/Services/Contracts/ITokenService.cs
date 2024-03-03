using CalendarApp.Models.Dtos.Responses;

namespace CalendarApp.Api.Services.Contracts;

public interface ITokenService
{
    string CreateToken(UserDto user);
}