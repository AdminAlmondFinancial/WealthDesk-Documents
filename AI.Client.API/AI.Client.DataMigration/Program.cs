using System.Reflection;
using DbUp;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
    ?? (args.Length > 0 ? args[0] : null);

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("No connection string. Set ConnectionStrings__Default or pass it as the first argument.");
    return 1;
}

EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.Error.WriteLine(result.Error);
    return 1;
}

Console.WriteLine("Database is up to date.");
return 0;
