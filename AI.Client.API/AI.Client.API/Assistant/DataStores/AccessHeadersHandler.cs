using Microsoft.Extensions.Options;

namespace AI.Client.API.Assistant.DataStores;

/// <summary>Adds both Cloudflare Access service-token headers to every orchestrator request.</summary>
public sealed class AccessHeadersHandler(IOptions<OrchestratorOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Remove("CF-Access-Client-Id");
        request.Headers.Remove("CF-Access-Client-Secret");
        request.Headers.Add("CF-Access-Client-Id", options.Value.AccessClientId);
        request.Headers.Add("CF-Access-Client-Secret", options.Value.AccessClientSecret);
        return base.SendAsync(request, cancellationToken);
    }
}
