namespace AI.Client.API.Assistant.DataStores;

public interface IOrchestratorDataStore
{
    Task<FormatOutcome> FormatAsync(FormatRequest request, CancellationToken ct);
}
