namespace CalendarApp.Api.Entities;

public class ScheduledClass : IEntity
{
    public uint Id { get; set; }
    public uint SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCancelled { get; set; }
}