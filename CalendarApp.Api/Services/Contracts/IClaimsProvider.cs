using System.Security.Claims;

namespace CalendarApp.Api.Services.Contracts;

public interface IClaimsProvider
{
    int? GetUserIdOrDefault(ClaimsPrincipal claimsPrincipal);
    int GetUserId(ClaimsPrincipal claimsPrincipal);
}