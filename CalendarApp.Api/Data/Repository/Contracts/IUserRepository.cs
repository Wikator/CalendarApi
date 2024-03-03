using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;

namespace CalendarApp.Api.Data.Repository.Contracts;

public interface IUserRepository
{
    UserDto Register(User user);
    Task<UserDto?> LoginAsync(string username, string password);
}