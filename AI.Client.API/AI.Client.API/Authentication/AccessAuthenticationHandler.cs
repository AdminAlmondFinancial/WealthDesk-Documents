using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AI.Client.API.Authentication;

public sealed class AccessAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, AccessTokenValidator validator)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "CloudflareAccess";
    public const string TokenHeader = "Cf-Access-Jwt-Assertion";
    public const string ForwardedEmailHeader = "X-User-Email";
    public const string ForwardedNameHeader = "X-User-Name";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(TokenHeader, out var token)) return AuthenticateResult.NoResult();

        AccessIdentity? identity;
        try
        {
            identity = await validator.ValidateAsync(
                token, Request.Headers[ForwardedEmailHeader], Request.Headers[ForwardedNameHeader], Context.RequestAborted);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            // Never log the token or the headers.
            Logger.LogWarning("Could not load Cloudflare Access signing keys: {ExceptionType}", e.GetType().Name);
            return AuthenticateResult.Fail("Could not verify the Access token.");
        }

        if (identity is null) return AuthenticateResult.Fail("Invalid Access token.");

        var claims = new ClaimsIdentity(
            [new Claim("email", identity.Email), new Claim("name", identity.Name)], SchemeName, "name", ClaimTypes.Role);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(claims), SchemeName));
    }
}
