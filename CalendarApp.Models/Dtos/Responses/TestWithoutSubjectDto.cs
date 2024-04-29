namespace CalendarApp.Models.Dtos.Responses;

public class TestWithoutSubjectDto
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}