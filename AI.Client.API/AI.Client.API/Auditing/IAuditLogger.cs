namespace AI.Client.API.Auditing;

public interface IAuditLogger
{
    /// <summary>Records who did what, when, with the status code and duration. Never document content.</summary>
    Task RecordAsync(string userId, string action, int statusCode, long durationMs, CancellationToken ct);
}
