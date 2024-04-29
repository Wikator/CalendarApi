using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class RegisterDto
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    [Required, Range(1, 4)] public int? Group { get; init; }
    public int? Faculty1 { get; set; }
    public int? Faculty2 { get; set; }
    public int? Faculty3 { get; set; }
}