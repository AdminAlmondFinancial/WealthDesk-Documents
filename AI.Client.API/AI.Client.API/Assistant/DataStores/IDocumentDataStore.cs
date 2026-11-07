namespace AI.Client.API.Assistant.DataStores;

public interface IDocumentDataStore
{
    Task AddAsync(IFormattedDocument document, CancellationToken ct);
    Task<IFormattedDocument?> GetForUserAsync(Guid id, string userId, CancellationToken ct);
}
