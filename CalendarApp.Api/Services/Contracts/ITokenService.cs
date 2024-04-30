using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Dtos.Responses.User;

namespace CalendarApp.Api.Services.Contracts;

public interface ITokenService
{
    string CreateToken(UserDto user);
}