using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.DataAccess.Repository;

public class UserRepository(DbContext dbContext, IMapper mapper) : IUserRepository
{
    private DbSet<User> Users { get; } = dbContext.Set<User>();
    private IMapper Mapper { get; } = mapper;

    public void Register(User user)
    {
        Users.Add(user);
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var user = await Users.FirstOrDefaultAsync(x => x.Username == username);

        if (user is null)
            return null;

        using HMACSHA512 hmac = new(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.Where((t, i) => t != user.PasswordHash[i]).Any() ? null : Mapper.Map<UserDto>(user);
    }
}