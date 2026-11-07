namespace AI.Client.API.Assistant.DataStores;

public interface IChatDataStore
{
    Task<IReadOnlyList<IChatSession>> ListForUserAsync(string userId, CancellationToken ct);
    Task AddAsync(IChatSession chat, CancellationToken ct);
}
