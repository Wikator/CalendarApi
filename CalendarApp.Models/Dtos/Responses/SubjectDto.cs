namespace CalendarApp.Models.Dtos.Responses;

public class SubjectDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int FacultyType { get; init; }
    public ICollection<TestWithoutSubjectDto> Tests { get; init; } = new List<TestWithoutSubjectDto>();
}