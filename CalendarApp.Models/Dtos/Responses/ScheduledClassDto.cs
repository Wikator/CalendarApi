namespace CalendarApp.Models.Dtos.Responses;

public class ScheduledClassDto
{
    public uint Id { get; init; }
    public required SubjectDto Subject { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required TimeSpan Duration { get; init; }
}