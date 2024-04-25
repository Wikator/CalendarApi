namespace CalendarApp.Models.Entities;

public interface IAuthorizedEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
}