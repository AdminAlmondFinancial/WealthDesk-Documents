using System.Net;
using System.Text;
using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Client.UnitTests.Assistant.DataStores;

public class OrchestratorDataStoreTests
{
    private const string Secret = "SECRET-DOCUMENT-TEXT";
    private const string OkBody = """{"result":"Formatted","seconds":1.5,"mode":"x","warnings":["check totals"]}""";

    [Fact]
    public async Task BadGatewayThenOk_FormatAsync_RetriesOnceAndSucceeds()
    {
        var (store, handler, _) = Create(HttpStatusCode.BadGateway, HttpStatusCode.OK);
        var outcome = await store.FormatAsync(new FormatRequest(Secret), CancellationToken.None);
        Assert.Equal(2, handler.Calls);
        Assert.Equal("Formatted", outcome.FormattedText);
        Assert.Equal(["check totals"], outcome.Warnings);
    }

    [Fact]
    public async Task BadGatewayTwice_FormatAsync_StopsAfterOneRetry()
    {
        var (store, handler, _) = Create(HttpStatusCode.BadGateway, HttpStatusCode.BadGateway, HttpStatusCode.OK);
        await Assert.ThrowsAsync<OrchestratorUnavailableException>(() => store.FormatAsync(new FormatRequest(Secret), CancellationToken.None));
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public async Task UnprocessableEntity_FormatAsync_IsNeverRetried()
    {
        var (store, handler, _) = Create(HttpStatusCode.UnprocessableEntity, HttpStatusCode.OK);
        await Assert.ThrowsAsync<OrchestratorRejectedException>(() => store.FormatAsync(new FormatRequest(Secret), CancellationToken.None));
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task AnyCall_FormatAsync_LogsStatusAndTimingButNoDocumentText()
    {
        var (store, _, logger) = Create(HttpStatusCode.OK);
        await store.FormatAsync(new FormatRequest(Secret), CancellationToken.None);
        Assert.NotEmpty(logger.Lines);
        Assert.All(logger.Lines, l => Assert.DoesNotContain(Secret, l));
        Assert.Contains(logger.Lines, l => l.Contains("200") && l.Contains("ms"));
    }

    [Fact]
    public async Task NonStringResult_FormatAsync_ThrowsUnavailable()
    {
        var (store, _, _) = Create(HttpStatusCode.OK, body: """{"result":{"a":1},"seconds":1}""");
        await Assert.ThrowsAsync<OrchestratorUnavailableException>(() => store.FormatAsync(new FormatRequest("x"), CancellationToken.None));
    }

    private static (OrchestratorDataStore, StubHandler, CapturingLogger) Create(params HttpStatusCode[] statuses) =>
        Create(statuses, OkBody);

    private static (OrchestratorDataStore, StubHandler, CapturingLogger) Create(HttpStatusCode status, string body) =>
        Create([status], body);

    private static (OrchestratorDataStore, StubHandler, CapturingLogger) Create(HttpStatusCode[] statuses, string body)
    {
        var handler = new StubHandler(statuses, body);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://worker.test/") };
        var logger = new CapturingLogger();
        var options = Options.Create(new OrchestratorOptions { RetryDelaySeconds = 0 });
        return (new OrchestratorDataStore(http, options, logger), handler, logger);
    }

    private sealed class StubHandler(HttpStatusCode[] statuses, string body) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var status = statuses[Math.Min(Calls++, statuses.Length - 1)];
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(status == HttpStatusCode.OK ? body : "{}", Encoding.UTF8, "application/json"),
            });
        }
    }

    private sealed class CapturingLogger : ILogger<OrchestratorDataStore>
    {
        public List<string> Lines { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex, Func<TState, Exception?, string> f) =>
            Lines.Add(f(state, ex));
    }
}
