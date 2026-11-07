namespace AI.Client.API.Authentication;

public sealed class AccessOptions
{
    public const string Section = "Access";

    // Cloudflare Access team domain, e.g. almondfinancial.cloudflareaccess.com
    public string TeamDomain { get; set; } = string.Empty;

    // AUD tag of the Access application that protects THIS backend (not the Worker's).
    public string Audience { get; set; } = string.Empty;

    // Client id of the Worker's service token (wealthdesk-microservice). Only this caller may forward a user's email.
    public string ServiceTokenClientId { get; set; } = string.Empty;
}
