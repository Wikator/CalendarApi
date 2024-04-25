using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class LoginDto
{
    [Required] public string Username { get; init; } = "";
    [Required] public string Password { get; init; } = "";
}