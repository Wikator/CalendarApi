namespace CalendarApp.Models.Dtos.Responses.Test;

public class TestWithoutSubjectDto
{
    public int Id { get; init; }
    public int? Group { get; init; }
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}