using System.Security.Claims;
using CalendarApp.Api.Services.Contracts;

namespace CalendarApp.Api.Services;

public class ClaimsProvider : IClaimsProvider
{
    public uint? GetUserIdOrDefault(ClaimsPrincipal claimsPrincipal)
    {
        var claimValue = claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return claimValue switch
        {
            null => null,
            _ => uint.Parse(claimValue)
        };
    }

    public uint GetUserId(ClaimsPrincipal claimsPrincipal) =>
        uint.Parse(claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
}