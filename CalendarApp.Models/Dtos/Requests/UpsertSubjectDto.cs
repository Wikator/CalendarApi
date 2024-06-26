using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Models.Dtos.Requests;

public class UpsertSubjectDto
{
    [Required] public string Name { get; set; } = "";

    [Range(0, 2)] public int FacultyType { get; set; }
}