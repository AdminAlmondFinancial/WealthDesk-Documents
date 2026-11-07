using System.Security.Cryptography;
using AI.Client.API.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AI.Client.UnitTests.Authentication;

public class AccessTokenValidatorTests
{
    private const string Team = "almondfinancial.cloudflareaccess.com";
    private const string Aud = "backend-aud";
    private const string ServiceClient = "worker-token-id";

    private readonly RsaSecurityKey _signing = new(RSA.Create(2048)) { KeyId = "k1" };
    private readonly AccessTokenValidator _validator;

    public AccessTokenValidatorTests()
    {
        var options = Options.Create(new AccessOptions { TeamDomain = Team, Audience = Aud, ServiceTokenClientId = ServiceClient });
        _validator = new AccessTokenValidator(options, new FakeKeys(_signing));
    }

    [Fact]
    public async Task UserToken_ValidateAsync_ReturnsTheTokenEmailLowercased()
    {
        var id = await _validator.ValidateAsync(Token(new() { ["email"] = "Sarah@Almond.com" }), null, null, default);
        Assert.Equal("sarah@almond.com", id!.Email);
    }

    [Fact]
    public async Task UserToken_ValidateAsync_IgnoresAForwardedEmail()
    {
        var id = await _validator.ValidateAsync(Token(new() { ["email"] = "real@almond.com" }), "other@almond.com", null, default);
        Assert.Equal("real@almond.com", id!.Email);
    }

    [Fact]
    public async Task WrongAudience_ValidateAsync_ReturnsNull() =>
        Assert.Null(await _validator.ValidateAsync(Token(new() { ["email"] = "a@b.com" }, audience: "someone-elses-app"), null, null, default));

    [Fact]
    public async Task ExpiredToken_ValidateAsync_ReturnsNull() =>
        Assert.Null(await _validator.ValidateAsync(Token(new() { ["email"] = "a@b.com" }, expires: DateTime.UtcNow.AddHours(-1)), null, null, default));

    [Fact]
    public async Task TokenSignedWithAnotherKey_ValidateAsync_ReturnsNull()
    {
        var forged = Token(new() { ["email"] = "a@b.com" }, key: new RsaSecurityKey(RSA.Create(2048)) { KeyId = "k1" });
        Assert.Null(await _validator.ValidateAsync(forged, null, null, default));
    }

    [Fact]
    public async Task WorkerServiceToken_ValidateAsync_TrustsTheForwardedEmail()
    {
        var id = await _validator.ValidateAsync(Token(new() { ["common_name"] = ServiceClient }), "sarah@almond.com", "Sarah Thompson", default);
        Assert.Equal(new AccessIdentity("sarah@almond.com", "Sarah Thompson"), id);
    }

    [Fact]
    public async Task OtherServiceToken_ValidateAsync_IgnoresTheForwardedEmail() =>
        Assert.Null(await _validator.ValidateAsync(Token(new() { ["common_name"] = "someone-else" }), "sarah@almond.com", null, default));

    [Fact]
    public async Task WorkerServiceTokenWithoutEmail_ValidateAsync_ReturnsNull() =>
        Assert.Null(await _validator.ValidateAsync(Token(new() { ["common_name"] = ServiceClient }), null, null, default));

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("Name <a@b.com>")]
    public async Task WorkerServiceTokenWithBadEmail_ValidateAsync_ReturnsNull(string email) =>
        Assert.Null(await _validator.ValidateAsync(Token(new() { ["common_name"] = ServiceClient }), email, null, default));

    [Fact]
    public async Task MissingToken_ValidateAsync_ReturnsNull() =>
        Assert.Null(await _validator.ValidateAsync(null, "a@b.com", null, default));

    private string Token(Dictionary<string, object> claims, string audience = Aud, DateTime? expires = null, SecurityKey? key = null) =>
        new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = $"https://{Team}",
            Audience = audience,
            Expires = expires ?? DateTime.UtcNow.AddHours(1),
            NotBefore = expires?.AddHours(-1),
            Claims = claims,
            SigningCredentials = new SigningCredentials(key ?? _signing, SecurityAlgorithms.RsaSha256),
        });

    private sealed class FakeKeys(SecurityKey key) : IAccessSigningKeys
    {
        public Task<IReadOnlyCollection<SecurityKey>> GetAsync(bool refresh, CancellationToken ct) =>
            Task.FromResult<IReadOnlyCollection<SecurityKey>>([key]);
    }
}
