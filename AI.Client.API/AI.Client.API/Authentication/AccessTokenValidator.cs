using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AI.Client.API.Authentication;

public sealed record AccessIdentity(string Email, string Name);

/// <summary>
/// Verifies the signed Cf-Access-Jwt-Assertion token. A user token carries the email itself.
/// A service-token call (the Worker) carries no email, so the forwarded email is trusted only
/// when the verified token belongs to the configured service token.
/// </summary>
public sealed class AccessTokenValidator(IOptions<AccessOptions> options, IAccessSigningKeys keys)
{
    public async Task<AccessIdentity?> ValidateAsync(string? token, string? forwardedEmail, string? forwardedName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var jwt = await ValidateTokenAsync(token, refresh: false, ct) ?? await ValidateTokenAsync(token, refresh: true, ct);
        if (jwt is null) return null;

        if (jwt.TryGetPayloadValue<string>("email", out var email) && IsEmail(email))
            return Identity(email, null);

        var configuredClient = options.Value.ServiceTokenClientId;
        if (!string.IsNullOrEmpty(configuredClient)
            && jwt.TryGetPayloadValue<string>("common_name", out var client)
            && string.Equals(client, configuredClient, StringComparison.Ordinal)
            && IsEmail(forwardedEmail))
            return Identity(forwardedEmail!, forwardedName);

        return null;
    }

    private async Task<JsonWebToken?> ValidateTokenAsync(string token, bool refresh, CancellationToken ct)
    {
        var o = options.Value;
        var parameters = new TokenValidationParameters
        {
            ValidIssuer = $"https://{o.TeamDomain}",
            ValidAudience = o.Audience,
            IssuerSigningKeys = await keys.GetAsync(refresh, ct),
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ClockSkew = TimeSpan.FromMinutes(1),
        };
        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, parameters);
        return result.IsValid ? (JsonWebToken)result.SecurityToken : null;
    }

    private static bool IsEmail(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.Length <= 254
        && MailAddress.TryCreate(value, out var parsed) && parsed.Address == value;

    private static AccessIdentity Identity(string email, string? name)
    {
        var clean = email.ToLowerInvariant();
        var display = string.IsNullOrWhiteSpace(name) || name.Length > 100 ? clean.Split('@')[0] : name.Trim();
        return new AccessIdentity(clean, display);
    }
}
