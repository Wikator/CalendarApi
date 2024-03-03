using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IUserRepository
{
    UserDto Register(User user);
    Task<UserDto?> LoginAsync(string username, string password);
}