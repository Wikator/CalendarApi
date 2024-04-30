using CalendarApp.Models.Dtos.Responses.Subject;

namespace CalendarApp.Models.Dtos.Responses.Test;

public class TestDetailsDto : TestWithoutSubjectDto
{
    public required SubjectDto Subject { get; init; }
}