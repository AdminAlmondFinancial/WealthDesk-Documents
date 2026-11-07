using Dapper;
using Npgsql;

namespace AI.Client.API.Auditing;

public sealed class AuditLogger(NpgsqlDataSource dataSource, TimeProvider time) : IAuditLogger
{
    private const string InsertSql =
        "INSERT INTO audit_log (user_id, action, status_code, duration_ms, created_utc) VALUES (@userId, @action, @statusCode, @durationMs, @createdUtc)";

    public async Task RecordAsync(string userId, string action, int statusCode, long durationMs, CancellationToken ct)
    {
        await using var connection = await dataSource.OpenConnectionAsync(ct);
        await connection.ExecuteAsync(new CommandDefinition(
            InsertSql,
            new { userId, action, statusCode, durationMs = (int)durationMs, createdUtc = time.GetUtcNow().UtcDateTime },
            cancellationToken: ct));
    }
}
