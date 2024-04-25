namespace CalendarApp.Models.Dtos.Requests;

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public int? Faculty1 { get; set; }
    public int? Faculty2 { get; set; }
    public int? Faculty3 { get; set; }
}