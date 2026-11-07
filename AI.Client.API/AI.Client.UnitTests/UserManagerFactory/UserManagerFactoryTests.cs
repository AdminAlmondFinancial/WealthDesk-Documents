using System.Security.Claims;
using AI.Client.API.AccountUsers;
using AI.Client.API.Validation;

namespace AI.Client.UnitTests.UserManagerFactory;

public class UserManagerFactoryTests
{
    private static ClaimsPrincipal Principal(params Claim[] claims) => new(new ClaimsIdentity(claims, "test"));

    [Fact]
    public void VerifiedEmail_Create_UsesItAsIdAndGrantsAllAreas()
    {
        var user = new AI.Client.API.UserManagerFactory.UserManagerFactory()
            .Create(Principal(new Claim("email", "sarah@almond.com"), new Claim("name", "Sarah")));
        Assert.Equal("sarah@almond.com", user.Id);
        Assert.Equal(AccessAreas.All, user.Areas);
    }

    [Fact]
    public void NoEmail_Create_ThrowsForbidden() =>
        Assert.Throws<ForbiddenException>(() =>
            new AI.Client.API.UserManagerFactory.UserManagerFactory().Create(Principal(new Claim("name", "X"))));
}
