using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AI.Client.API.Authentication;

public static class EntraAuthenticationExtensions
{
    /// <summary>Validates Microsoft Entra ID bearer tokens ("Continue with Microsoft").</summary>
    public static IServiceCollection AddEntraAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var entra = configuration.GetSection(EntraOptions.Section).Get<EntraOptions>() ?? new EntraOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://login.microsoftonline.com/{entra.TenantId}/v2.0";
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new[] { entra.ClientId, $"api://{entra.ClientId}" },
                    NameClaimType = "name",
                };
            });
        services.AddAuthorization();
        return services;
    }
}
