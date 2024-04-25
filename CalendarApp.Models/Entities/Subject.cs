namespace CalendarApp.Models.Entities;

public class Subject : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int FacultyType { get; set; }
    public ICollection<ScheduledClass> ScheduledClasses { get; set; } = [];
}