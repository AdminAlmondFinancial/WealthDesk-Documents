using Microsoft.AspNetCore.Authentication;

namespace AI.Client.API.Authentication;

public static class AccessAuthenticationExtensions
{
    /// <summary>Trusts only requests carrying a valid Cloudflare Access token for this backend's Access application.</summary>
    public static IServiceCollection AddCloudflareAccessAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AccessOptions>()
            .Bind(configuration.GetSection(AccessOptions.Section))
            .Validate(o => !string.IsNullOrWhiteSpace(o.TeamDomain) && !string.IsNullOrWhiteSpace(o.Audience),
                "Access TeamDomain and Audience are required.")
            .ValidateOnStart();

        services.AddHttpClient<IAccessSigningKeys, AccessSigningKeys>();
        services.AddSingleton<AccessTokenValidator>();
        services.AddAuthentication(AccessAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, AccessAuthenticationHandler>(AccessAuthenticationHandler.SchemeName, _ => { });
        services.AddAuthorization();
        return services;
    }
}
