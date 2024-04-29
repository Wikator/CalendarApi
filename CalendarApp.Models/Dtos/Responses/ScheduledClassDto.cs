namespace CalendarApp.Models.Dtos.Responses;

public class ScheduledClassDto
{
    public int Id { get; init; }
    public int Group { get; init; }
    public required SubjectDto Subject { get; set; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public ICollection<NoteDto> Notes { get; init; } = new List<NoteDto>();
}