using System.Security.Claims;
using CalendarApp.Api.Services.Contracts;

namespace CalendarApp.Api.Services;

public class ClaimsProvider : IClaimsProvider
{
    public int? GetUserIdOrDefault(ClaimsPrincipal claimsPrincipal)
    {
        var claimValue = claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return claimValue switch
        {
            null => null,
            _ => int.Parse(claimValue)
        };
    }

    public int GetUserId(ClaimsPrincipal claimsPrincipal) =>
        int.Parse(claimsPrincipal.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
}