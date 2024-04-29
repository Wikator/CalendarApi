using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Entities;

public class User : IEntity
{
    public int Id { get; set; }
    public required string Username { get; init; }
    public int Group { get; init; }
    public string Role { get; init; } = "User";
    public required byte[] PasswordHash { get; init; }
    public required byte[] PasswordSalt { get; init; }
    public int? Faculty1Id { get; init; }
    public Subject? Faculty1 { get; init; }
    public int? Faculty2Id { get; init; }
    public Subject? Faculty2 { get; init; }
    public int? Faculty3Id { get; init; }
    public Subject? Faculty3 { get; init; }
}