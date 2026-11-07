namespace AI.Client.API.Assistant.DataStores;

public sealed class OrchestratorOptions
{
    public const string Section = "Orchestrator";

    // The wealthdesk-documents Worker, e.g. https://wealthdesk-documents.<account>.workers.dev
    public string BaseUrl { get; set; } = string.Empty;

    // Cloudflare Access service token. Supplied by secret settings (Key Vault), never committed.
    public string AccessClientId { get; set; } = string.Empty;
    public string AccessClientSecret { get; set; } = string.Empty;

    public int TimeoutMinutes { get; set; } = 10;
    public int RetryDelaySeconds { get; set; } = 2;
}
