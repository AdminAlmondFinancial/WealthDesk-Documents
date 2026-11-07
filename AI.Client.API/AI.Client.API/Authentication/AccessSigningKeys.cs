using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AI.Client.API.Authentication;

public sealed class AccessSigningKeys(HttpClient http, IOptions<AccessOptions> options, TimeProvider time) : IAccessSigningKeys
{
    private static readonly TimeSpan MaxAge = TimeSpan.FromHours(1);
    private static readonly TimeSpan MinRefresh = TimeSpan.FromMinutes(1);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyCollection<SecurityKey> _keys = [];
    private DateTimeOffset _fetchedAt = DateTimeOffset.MinValue;

    public async Task<IReadOnlyCollection<SecurityKey>> GetAsync(bool refresh, CancellationToken ct)
    {
        var now = time.GetUtcNow();
        var age = now - _fetchedAt;
        if (_keys.Count > 0 && age < MaxAge && !(refresh && age >= MinRefresh)) return _keys;

        await _gate.WaitAsync(ct);
        try
        {
            now = time.GetUtcNow();
            age = now - _fetchedAt;
            if (_keys.Count > 0 && age < MaxAge && !(refresh && age >= MinRefresh)) return _keys;

            var json = await http.GetStringAsync($"https://{options.Value.TeamDomain}/cdn-cgi/access/certs", ct);
            _keys = new JsonWebKeySet(json).GetSigningKeys().ToList();
            _fetchedAt = now;
            return _keys;
        }
        finally { _gate.Release(); }
    }
}
