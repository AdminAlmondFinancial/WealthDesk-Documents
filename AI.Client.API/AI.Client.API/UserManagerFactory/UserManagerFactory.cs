using System.Security.Claims;
using AI.Client.API.AccountUsers;
using AI.Client.API.Validation;

namespace AI.Client.API.UserManagerFactory;

public sealed class UserManagerFactory : IUserManagerFactory
{
    public CurrentUser Create(ClaimsPrincipal principal)
    {
        // The verified Cloudflare Access email is the user id (chat history is keyed by it).
        var id = principal.FindFirstValue("email") ?? throw new ForbiddenException();
        var name = principal.FindFirstValue("name") ?? string.Empty;
        var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        // Every user is granted all areas on login.
        return new CurrentUser(id, name, role, AccessAreas.All);
    }
}
