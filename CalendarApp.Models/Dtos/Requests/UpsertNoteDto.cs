namespace CalendarApp.Models.Dtos.Requests;

public class UpsertNoteDto
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}