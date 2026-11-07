using System.Diagnostics;
using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Assistant.Manager;
using AI.Client.API.Auditing;
using AI.Client.API.Authentication;
using AI.Client.API.Automapper;
using AI.Client.API.UserManagerFactory;
using AI.Client.API.Validation;
using Microsoft.Extensions.Options;
using Npgsql;

public sealed class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEntraAuthentication(configuration);

        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = configuration["AutoMapper:LicenseKey"];
            cfg.AddProfile<AssistantProfile>();
        });

        services.AddCors(o => o.AddDefaultPolicy(p => p
            .WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader().AllowAnyMethod()));

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(_ => NpgsqlDataSource.Create(configuration.GetConnectionString("Default") ?? string.Empty));

        services.AddOptions<OrchestratorOptions>()
            .Bind(configuration.GetSection(OrchestratorOptions.Section))
            .Validate(o => string.IsNullOrEmpty(o.BaseUrl)
                           || (!string.IsNullOrEmpty(o.AccessClientId) && !string.IsNullOrEmpty(o.AccessClientSecret)),
                "Orchestrator Access service token (AccessClientId and AccessClientSecret) is required.")
            .ValidateOnStart();

        services.AddTransient<AccessHeadersHandler>();
        services.AddHttpClient<IOrchestratorDataStore, OrchestratorDataStore>((sp, client) =>
            {
                var o = sp.GetRequiredService<IOptions<OrchestratorOptions>>().Value;
                if (!string.IsNullOrEmpty(o.BaseUrl)) client.BaseAddress = new Uri(o.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromMinutes(o.TimeoutMinutes);
            })
            .AddHttpMessageHandler<AccessHeadersHandler>();

        services.AddSingleton<IChatDataStore, ChatDataStore>();
        services.AddSingleton<IDocumentDataStore, DocumentDataStore>();
        services.AddSingleton<IDocumentConverter, DocumentConverter>();
        services.AddSingleton<IAuditLogger, AuditLogger>();
        services.AddSingleton<IUserManagerFactory, UserManagerFactory>();
        services.AddScoped<IAssistantManager, AssistantManager>();
    }

    public void Configure(WebApplication app)
    {
        app.Use(MapErrors);
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.Use(Audit);
        app.MapControllers();
    }

    // Generic messages only: never echo document content back or into logs.
    private static async Task MapErrors(HttpContext context, RequestDelegate next)
    {
        try { await next(context); }
        catch (Exception e) when (!context.Response.HasStarted)
        {
            var (status, message) = e switch
            {
                ValidationException v => (v.StatusCode, v.Message),
                ForbiddenException => (403, e.Message),
                NotFoundException => (404, e.Message),
                OrchestratorRejectedException => (422, e.Message),
                OrchestratorTimeoutException => (504, e.Message),
                OrchestratorUnavailableException => (502, "The document service is unavailable. Try again."),
                OperationCanceledException => (499, "Request cancelled."),
                _ => (500, "Something went wrong."),
            };
            if (status == 500)
                context.RequestServices.GetRequiredService<ILogger<Startup>>().LogError("Unhandled {ExceptionType}", e.GetType().Name);
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }

    // Who, what and when, with status code and timing. Never request or document content.
    private static async Task Audit(HttpContext context, RequestDelegate next)
    {
        var watch = Stopwatch.StartNew();
        await next(context);
        if (!context.Request.Path.StartsWithSegments("/api") || context.User.Identity?.IsAuthenticated != true) return;
        try
        {
            var user = context.RequestServices.GetRequiredService<IUserManagerFactory>().Create(context.User);
            await context.RequestServices.GetRequiredService<IAuditLogger>().RecordAsync(
                user.Id, $"{context.Request.Method} {context.Request.Path}", context.Response.StatusCode,
                watch.ElapsedMilliseconds, CancellationToken.None);
        }
        catch (Exception e)
        {
            context.RequestServices.GetRequiredService<ILogger<Startup>>().LogWarning("Audit write failed: {ExceptionType}", e.GetType().Name);
        }
    }
}
