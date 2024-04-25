namespace CalendarApp.Models.Entities;

public class Note : IAuthorizedEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; init; }
    public int ScheduledClassId { get; set; }
    public ScheduledClass? ScheduledClass { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public DateTime LastModified { get; init; }
}
