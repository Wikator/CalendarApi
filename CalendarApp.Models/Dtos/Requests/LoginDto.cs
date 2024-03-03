namespace CalendarApp.Models.Dtos.Requests;

public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}