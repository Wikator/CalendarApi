namespace CalendarApp.Models.Entities;

public class Subject : IEntity
{
    public int Id { get; set; }
    public required string Name { get; init; }
    public int FacultyType { get; init; }
    public ICollection<ScheduledClass> ScheduledClasses { get; init; } = [];
    public ICollection<Test> Tests { get; init; } = [];
}