using CalendarApp.Models.Dtos.Responses.User;
using CalendarApp.Models.Entities;

namespace CalendarApp.DataAccess.Repository.Contracts;

public interface IUserRepository
{
    void Register(User user);
    Task<UserDto?> LoginAsync(string username, string password);
    Task<int> GetGroupByUserIdAsync(int userId);
}