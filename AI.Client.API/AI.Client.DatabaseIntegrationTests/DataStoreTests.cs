using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Auditing;
using Dapper;

namespace AI.Client.DatabaseIntegrationTests;

public class DataStoreTests(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private static readonly DateTime Now = new(2026, 11, 7, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task AppliedMigrations_Schema_ContainsAllTables()
    {
        await using var connection = await db.DataSource.OpenConnectionAsync();
        var tables = (await connection.QueryAsync<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'")).ToList();
        Assert.Contains("chats", tables);
        Assert.Contains("formatted_documents", tables);
        Assert.Contains("audit_log", tables);
        Assert.DoesNotContain(tables, t => t.Contains("account"));
    }

    [Fact]
    public async Task SavedChats_ListForUserAsync_ReturnsOnlyThatUsersNewestFirst()
    {
        var store = new ChatDataStore(db.DataSource);
        await store.AddAsync(new ChatSession("db-user-a", "Older", Now.AddHours(-2)), default);
        await store.AddAsync(new ChatSession("db-user-a", "Newer", Now), default);
        await store.AddAsync(new ChatSession("db-user-b", "Not mine", Now), default);

        var list = await store.ListForUserAsync("db-user-a", default);

        Assert.Equal(["Newer", "Older"], list.Select(c => c.Title));
    }

    [Fact]
    public async Task SavedDocument_GetForUserAsync_RoundTripsContentAndHidesFromOtherUsers()
    {
        var store = new DocumentDataStore(db.DataSource);
        var doc = new FormattedDocument("db-user-a", "A (formatted).docx", "Word document", [1, 2, 3], Now);
        await store.AddAsync(doc, default);

        var found = await store.GetForUserAsync(doc.Id, "db-user-a", default);
        var hidden = await store.GetForUserAsync(doc.Id, "db-user-b", default);

        Assert.Equal([1, 2, 3], found!.Content);
        Assert.Null(hidden);
    }

    [Fact]
    public async Task AuditEntry_RecordAsync_StoresWhoWhatAndStatusOnly()
    {
        await new AuditLogger(db.DataSource, TimeProvider.System).RecordAsync("db-user-a", "GET /api/me", 200, 12, default);

        await using var connection = await db.DataSource.OpenConnectionAsync();
        var row = await connection.QuerySingleAsync("SELECT user_id, action, status_code, duration_ms FROM audit_log WHERE user_id = 'db-user-a'");
        Assert.Equal("GET /api/me", (string)row.action);
        Assert.Equal(200, (int)row.status_code);
    }
}
