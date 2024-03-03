using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CalendarApp.Api.Data.Repository.Contracts;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Api.Data.Repository;

public class UserRepository(DbContext dbContext, IMapper mapper) : IUserRepository
{
    private DbSet<User> Users { get; } = dbContext.Set<User>();
    private IMapper Mapper { get; } = mapper;

    public UserDto Register(User user)
    {
        Users.Add(user);
        return Mapper.Map<UserDto>(user);
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