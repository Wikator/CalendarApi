using CalendarApp.Models.Dtos.Responses.Note;
using CalendarApp.Models.Dtos.Responses.Subject;

namespace CalendarApp.Models.Dtos.Responses.ScheduledClass;

public sealed class ScheduledClassDetailsDto : ScheduledClassBaseDto
{
    public required SubjectDetailsDto Subject { get; init; }
    public required IReadOnlyCollection<NoteDto> Notes { get; init; }
}