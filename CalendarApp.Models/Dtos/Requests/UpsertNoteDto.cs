using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertNoteDto
{
    [Required] public string Title { get; init; } = "";
    [Required] public string Content { get; init; } = "";
}