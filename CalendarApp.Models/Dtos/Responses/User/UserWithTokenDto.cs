namespace CalendarApp.Models.Dtos.Responses.User;

public sealed class UserWithTokenDto : UserDto
{
    public required string Token { get; set; }
}