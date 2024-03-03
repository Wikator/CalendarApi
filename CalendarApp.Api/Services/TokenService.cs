using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace CalendarApp.Api.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private SymmetricSecurityKey Key { get; } = new(Encoding.UTF8.GetBytes(config["TokenKey"]
                                                                           ?? throw new Exception("Key not found")));

    public string CreateToken(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}