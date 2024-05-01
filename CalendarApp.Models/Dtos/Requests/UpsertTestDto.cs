using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public abstract class UpsertTestDto
{
    [Range(1, 4)] public int? Group { get; init; }
    [Required] public string? Title { get; init; }
    public string? Description { get; init; }
    [Required] public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}