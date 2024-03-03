using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertScheduledClassDto
{
    [Required] public uint SubjectId { get; init; }
    [Required] public DateTime StartTime { get; init; }
    [Required] public DateTime EndTime { get; init; }
    public bool IsCancelled { get; init; } = false;
}