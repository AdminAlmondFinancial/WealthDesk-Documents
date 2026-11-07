using Dapper;
using Npgsql;

namespace AI.Client.API.Assistant.DataStores;

public sealed class DocumentDataStore(NpgsqlDataSource dataSource) : IDocumentDataStore
{
    private const string InsertSql =
        "INSERT INTO formatted_documents (id, user_id, name, meta, content, created_utc) VALUES (@Id, @UserId, @Name, @Meta, @Content, @CreatedUtc)";

    private const string GetSql =
        "SELECT id, user_id AS UserId, name, meta, content, created_utc AS CreatedUtc FROM formatted_documents WHERE id = @id AND user_id = @userId";

    public async Task AddAsync(IFormattedDocument document, CancellationToken ct)
    {
        await using var connection = await dataSource.OpenConnectionAsync(ct);
        await connection.ExecuteAsync(new CommandDefinition(
            InsertSql,
            new { document.Id, document.UserId, document.Name, document.Meta, document.Content, document.CreatedUtc },
            cancellationToken: ct));
    }

    public async Task<IFormattedDocument?> GetForUserAsync(Guid id, string userId, CancellationToken ct)
    {
        await using var connection = await dataSource.OpenConnectionAsync(ct);
        var row = await connection.QuerySingleOrDefaultAsync<DocumentRow>(
            new CommandDefinition(GetSql, new { id, userId }, cancellationToken: ct));
        return row is null ? null : new FormattedDocument(row.Id, row.UserId, row.Name, row.Meta, row.Content, row.CreatedUtc);
    }

    private sealed record DocumentRow(Guid Id, string UserId, string Name, string Meta, byte[] Content, DateTime CreatedUtc);
}
