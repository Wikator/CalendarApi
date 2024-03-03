namespace CalendarApp.Models.Dtos.Requests;

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public uint? Faculty1 { get; set; }
    public uint? Faculty2 { get; set; }
    public uint? Faculty3 { get; set; }
}