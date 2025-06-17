using System.Security.Claims;

namespace ChatAgent.Api.Services.Privy;

public interface IPrivyService
{
    ClaimsPrincipal ValidateJwt(string jwt);
}