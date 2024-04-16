namespace CalendarApp.Models.Dtos.Responses;

public class SubjectDto
{
    public uint Id { get; init; }
    public required string Name { get; init; }
    public uint FacultyType { get; init; }
}