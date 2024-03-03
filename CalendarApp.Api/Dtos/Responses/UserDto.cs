namespace CalendarApp.Api.Dtos.Responses;

public class UserDto
{
    public uint Id { get; init; }
    public required string Username { get; init; }
    public required string Role { get; init; }
    public SubjectDto? Faculty1 { get; init; }
    public SubjectDto? Faculty2 { get; init; }
    public SubjectDto? Faculty3 { get; init; }

    public UserWithTokenDto ToUserWithTokenDto(string token)
    {
        return new UserWithTokenDto
        {
            Id = Id,
            Username = Username,
            Role = Role,
            Faculty1 = Faculty1,
            Faculty2 = Faculty2,
            Faculty3 = Faculty3,
            Token = token
        };
    }
}