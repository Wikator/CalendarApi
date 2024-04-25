using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertTestDto
{
    [Required] public int? SubjectId { get; init; }
    [Required] public string Title { get; init; } = "";
    public string? Description { get; init; }
    [Required] public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}