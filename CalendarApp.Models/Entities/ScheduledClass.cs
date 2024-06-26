using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Entities;

public class ScheduledClass : IEntity
{
    public int Id { get; set; }
    public int SubjectId { get; init; }
    public Subject? Subject { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool IsCancelled { get; init; }
    [Range(1, 4)] public int Group { get; init; }
    public ICollection<Note> Notes { get; init; } = new List<Note>();
}