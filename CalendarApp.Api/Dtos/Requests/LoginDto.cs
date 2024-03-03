namespace CalendarApp.Api.Dtos.Requests;

public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}