namespace CalendarApp.Api.Dtos.Responses;

public class UserWithTokenDto : UserDto
{
    public required string Token { get; set; }
}