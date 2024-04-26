using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class CreateTestDto : UpsertTestDto
{
    [Required] public int? SubjectId { get; init; }
}