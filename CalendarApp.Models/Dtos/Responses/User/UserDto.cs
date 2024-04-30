using CalendarApp.Models.Dtos.Responses.Subject;

namespace CalendarApp.Models.Dtos.Responses.User;

public class UserDto
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public required string Role { get; init; }
    public int Group { get; init; }
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
            Group = Group,
            Faculty1 = Faculty1,
            Faculty2 = Faculty2,
            Faculty3 = Faculty3,
            Token = token
        };
    }
}