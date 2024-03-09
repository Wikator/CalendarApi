using System.Security.Claims;

namespace CalendarApp.Api.Services.Contracts;

public interface IClaimsProvider
{
    uint? GetUserIdOrDefault(ClaimsPrincipal claimsPrincipal);
    uint GetUserId(ClaimsPrincipal claimsPrincipal);
}