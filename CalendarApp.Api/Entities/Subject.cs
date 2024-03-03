namespace CalendarApp.Api.Entities;

public class Subject : IEntity
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint FacultyType { get; set; }
    public ICollection<ScheduledClass> ScheduledClasses { get; set; } = [];
}