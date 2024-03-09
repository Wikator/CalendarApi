namespace CalendarApp.Models.Entities;

public interface IAuthorizedEntity
{
    public uint Id { get; set; }
    public uint UserId { get; set; }
}