namespace AI.Client.API.Authentication;

public sealed class EntraOptions
{
    public const string Section = "Entra";

    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
}
