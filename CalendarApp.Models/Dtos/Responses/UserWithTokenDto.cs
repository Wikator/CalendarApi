namespace CalendarApp.Models.Dtos.Responses;

public class UserWithTokenDto : UserDto
{
    public required string Token { get; set; }
}