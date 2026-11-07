using System.Security.Claims;

namespace AI.Client.API.UserManagerFactory;

public interface IUserManagerFactory
{
    CurrentUser Create(ClaimsPrincipal principal);
}
