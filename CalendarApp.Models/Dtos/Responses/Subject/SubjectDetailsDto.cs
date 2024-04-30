using CalendarApp.Models.Dtos.Responses.Test;

namespace CalendarApp.Models.Dtos.Responses.Subject;

public sealed class SubjectDetailsDto : SubjectDto
{
    public ICollection<TestWithoutSubjectDto> Tests { get; init; } = new List<TestWithoutSubjectDto>();
}