namespace CalendarApp.Models.Dtos.Responses;

public class TestDto : TestWithoutSubjectDto
{
    public required SubjectDto Subject { get; set; }
}