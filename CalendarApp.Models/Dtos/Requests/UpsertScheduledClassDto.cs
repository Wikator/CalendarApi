using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertScheduledClassDto
{
    [Required] public int? SubjectId { get; init; }
    [Required, Range(1, 4)] public int? Group { get; init; }
    [Required] public DateTime? StartTime { get; init; }
    [Required] public DateTime? EndTime { get; init; }
    public bool IsCancelled { get; init; } = false;
}