namespace CalendarApp.Models.Dtos.Responses.ScheduledClass;

public abstract class ScheduledClassBaseDto
{
    public int Id { get; init; }
    public int Group { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public bool IsCancelled { get; set; }
    public required TimeSpan Duration { get; init; }
}