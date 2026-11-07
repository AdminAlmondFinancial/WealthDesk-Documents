using Microsoft.IdentityModel.Tokens;

namespace AI.Client.API.Authentication;

public interface IAccessSigningKeys
{
    /// <summary>Cloudflare Access public signing keys. refresh asks for a re-fetch (limited to once a minute).</summary>
    Task<IReadOnlyCollection<SecurityKey>> GetAsync(bool refresh, CancellationToken ct);
}
