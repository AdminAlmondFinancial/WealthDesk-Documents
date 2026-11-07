using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Auditing;

namespace AI.Client.IntegrationTests;

public sealed class MockChatDataStore : IChatDataStore
{
    public List<IChatSession> Items { get; } = [];

    public Task<IReadOnlyList<IChatSession>> ListForUserAsync(string userId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<IChatSession>>(Items.Where(c => c.UserId == userId).OrderByDescending(c => c.CreatedUtc).ToList());

    public Task AddAsync(IChatSession chat, CancellationToken ct) { Items.Add(chat); return Task.CompletedTask; }
}

public sealed class MockDocumentDataStore : IDocumentDataStore
{
    public List<IFormattedDocument> Items { get; } = [];

    public Task AddAsync(IFormattedDocument document, CancellationToken ct) { Items.Add(document); return Task.CompletedTask; }

    public Task<IFormattedDocument?> GetForUserAsync(Guid id, string userId, CancellationToken ct) =>
        Task.FromResult(Items.FirstOrDefault(d => d.Id == id && d.UserId == userId));
}

public sealed class MockOrchestratorDataStore : IOrchestratorDataStore
{
    public int Calls { get; private set; }
    public Exception? Throw { get; set; }

    public Task<FormatOutcome> FormatAsync(FormatRequest request, CancellationToken ct)
    {
        Calls++;
        if (Throw is not null) throw Throw;
        return Task.FromResult(new FormatOutcome("Formatted text", 1, ["Check the totals"]));
    }
}

public sealed class MockAuditLogger : IAuditLogger
{
    public List<(string UserId, string Action, int Status)> Entries { get; } = [];

    public Task RecordAsync(string userId, string action, int statusCode, long durationMs, CancellationToken ct)
    {
        lock (Entries) Entries.Add((userId, action, statusCode));
        return Task.CompletedTask;
    }
}
