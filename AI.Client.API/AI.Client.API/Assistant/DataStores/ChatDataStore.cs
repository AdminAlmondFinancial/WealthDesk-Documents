using Dapper;
using Npgsql;

namespace AI.Client.API.Assistant.DataStores;

public sealed class ChatDataStore(NpgsqlDataSource dataSource) : IChatDataStore
{
    private const string ListSql =
        "SELECT id, user_id AS UserId, title, created_utc AS CreatedUtc FROM chats WHERE user_id = @userId ORDER BY created_utc DESC";

    private const string InsertSql =
        "INSERT INTO chats (id, user_id, title, created_utc) VALUES (@Id, @UserId, @Title, @CreatedUtc)";

    public async Task<IReadOnlyList<IChatSession>> ListForUserAsync(string userId, CancellationToken ct)
    {
        await using var connection = await dataSource.OpenConnectionAsync(ct);
        var rows = await connection.QueryAsync<ChatRow>(new CommandDefinition(ListSql, new { userId }, cancellationToken: ct));
        return rows.Select(r => (IChatSession)new ChatSession(r.Id, r.UserId, r.Title, r.CreatedUtc)).ToList();
    }

    public async Task AddAsync(IChatSession chat, CancellationToken ct)
    {
        await using var connection = await dataSource.OpenConnectionAsync(ct);
        await connection.ExecuteAsync(new CommandDefinition(
            InsertSql, new { chat.Id, chat.UserId, chat.Title, chat.CreatedUtc }, cancellationToken: ct));
    }

    private sealed record ChatRow(Guid Id, string UserId, string Title, DateTime CreatedUtc);
}
