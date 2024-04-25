namespace CalendarApp.Models.Dtos.Responses;

public class NoteDto
{
    public int Id { get; set; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public DateTime LastModified { get; init; }
}