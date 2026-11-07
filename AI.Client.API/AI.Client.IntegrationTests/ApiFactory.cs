using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Auditing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Client.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public MockChatDataStore Chats { get; } = new();
    public MockDocumentDataStore Documents { get; } = new();
    public MockOrchestratorDataStore Orchestrator { get; } = new();
    public MockAuditLogger Audit { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?> { ["Orchestrator:BaseUrl"] = "",
                ["Access:Audience"] = "test-audience" }));

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
            services.Configure<AuthenticationOptions>(o =>
            {
                o.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                o.DefaultChallengeScheme = TestAuthHandler.Scheme;
            });

            Replace<IChatDataStore>(services, Chats);
            Replace<IDocumentDataStore>(services, Documents);
            Replace<IOrchestratorDataStore>(services, Orchestrator);
            Replace<IAuditLogger>(services, Audit);
        });
    }

    public HttpClient CreateUserClient(string userId = "user-1", string? timeZone = null)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-User", userId);
        if (timeZone is not null) client.DefaultRequestHeaders.Add("X-Timezone", timeZone);
        return client;
    }

    private static void Replace<T>(IServiceCollection services, T instance) where T : class
    {
        foreach (var d in services.Where(d => d.ServiceType == typeof(T)).ToList()) services.Remove(d);
        services.AddSingleton(instance);
    }
}
