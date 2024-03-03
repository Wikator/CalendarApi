using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertSubjectDto
{
    public required string Name { get; set; }

    [Range(0, 2)] public uint FacultyType { get; set; }
}