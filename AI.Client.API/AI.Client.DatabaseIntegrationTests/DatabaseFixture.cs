using System.Reflection;
using DbUp;
using Npgsql;

namespace AI.Client.DatabaseIntegrationTests;

/// <summary>Creates a throwaway Postgres database, runs the real migrations, and drops it afterwards.</summary>
public sealed class DatabaseFixture : IDisposable
{
    private readonly string _adminConnection;
    private readonly string _databaseName = $"aiclient_tests_{Guid.NewGuid():N}";

    public DatabaseFixture()
    {
        _adminConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

        var builder = new NpgsqlConnectionStringBuilder(_adminConnection) { Database = _databaseName };
        ConnectionString = builder.ConnectionString;

        EnsureDatabase.For.PostgresqlDatabase(ConnectionString);
        var result = DeployChanges.To.PostgresqlDatabase(ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.Load(new AssemblyName("AI.Client.DataMigration")))
            .Build()
            .PerformUpgrade();
        if (!result.Successful) throw result.Error;

        DataSource = NpgsqlDataSource.Create(ConnectionString);
    }

    public string ConnectionString { get; }
    public NpgsqlDataSource DataSource { get; }

    public void Dispose()
    {
        DataSource.Dispose();
        NpgsqlConnection.ClearAllPools();
        using var admin = new NpgsqlConnection(_adminConnection);
        admin.Open();
        using var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_databaseName}\" WITH (FORCE)", admin);
        cmd.ExecuteNonQuery();
    }
}
