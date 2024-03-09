namespace CalendarApp.Models.Entities;

public class Note : IAuthorizedEntity
{
    public uint Id { get; set; }
    public uint UserId { get; set; }
    public User? User { get; init; }
    public uint ScheduledClassId { get; set; }
    public ScheduledClass? ScheduledClass { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public DateTime LastModified { get; init; }
}
