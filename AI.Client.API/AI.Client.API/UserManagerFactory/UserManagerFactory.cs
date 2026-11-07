using System.Security.Claims;
using AI.Client.API.AccountUsers;
using AI.Client.API.Validation;

namespace AI.Client.API.UserManagerFactory;

public sealed class UserManagerFactory : IUserManagerFactory
{
    public CurrentUser Create(ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue("oid")
                 ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? principal.FindFirstValue("sub")
                 ?? throw new ForbiddenException();
        var name = principal.FindFirstValue("name") ?? principal.Identity?.Name ?? string.Empty;
        var role = principal.FindFirstValue("roles") ?? principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        // Every user is granted all areas on login.
        return new CurrentUser(id, name, role, AccessAreas.All);
    }
}
