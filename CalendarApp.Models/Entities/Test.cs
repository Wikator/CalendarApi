using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Entities;

public class Test : IEntity
{
    public int Id { get; set; }
    [Range(1, 4)] public int? Group { get; init; }
    public int SubjectId { get; init; }
    public Subject? Subject { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}
