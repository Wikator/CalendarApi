namespace CalendarApp.Api.Entities;

public class User : IEntity
{
    public uint Id { get; set; }
    public required string Username { get; init; }
    public string Role { get; init; } = "User";
    public required byte[] PasswordHash { get; init; }
    public required byte[] PasswordSalt { get; init; }
    public uint? Faculty1Id { get; init; }
    public Subject? Faculty1 { get; init; }
    public uint? Faculty2Id { get; init; }
    public Subject? Faculty2 { get; init; }
    public uint? Faculty3Id { get; init; }
    public Subject? Faculty3 { get; init; }
}