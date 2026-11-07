using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace AI.Client.API.Assistant.DataStores;

public sealed class OrchestratorDataStore(
    HttpClient http, IOptions<OrchestratorOptions> options, ILogger<OrchestratorDataStore> logger) : IOrchestratorDataStore
{
    private const string RunPath = "api/ai/run";

    public async Task<FormatOutcome> FormatAsync(FormatRequest request, CancellationToken ct)
    {
        using var response = await SendAsync("format", () => new HttpRequestMessage(HttpMethod.Post, RunPath)
        {
            Content = JsonContent.Create(new RunRequest("format", request.Text)),
        }, ct);

        var body = await response.Content.ReadFromJsonAsync<RunResponse>(ct)
                   ?? throw new OrchestratorUnavailableException("The orchestrator returned an empty reply.");
        if (body.Result.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(body.Result.GetString()))
            throw new OrchestratorUnavailableException("The orchestrator returned no formatted text.");

        return new FormatOutcome(body.Result.GetString()!, body.Seconds, body.Warnings ?? []);
    }

    // One retry on 502 only, after a short backoff. 422 is never retried.
    // Only timings and status codes are logged, never document text.
    private async Task<HttpResponseMessage> SendAsync(string operation, Func<HttpRequestMessage> build, CancellationToken ct)
    {
        for (var attempt = 1; ; attempt++)
        {
            var watch = Stopwatch.StartNew();
            HttpResponseMessage response;
            try
            {
                using var message = build();
                response = await http.SendAsync(message, ct);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning("Orchestrator {Operation} timed out after {ElapsedMs} ms (attempt {Attempt})",
                    operation, watch.ElapsedMilliseconds, attempt);
                throw new OrchestratorTimeoutException();
            }
            catch (HttpRequestException)
            {
                logger.LogWarning("Orchestrator {Operation} unreachable after {ElapsedMs} ms (attempt {Attempt})",
                    operation, watch.ElapsedMilliseconds, attempt);
                throw new OrchestratorUnavailableException("The orchestrator could not be reached.");
            }

            logger.LogInformation("Orchestrator {Operation} returned {StatusCode} in {ElapsedMs} ms (attempt {Attempt})",
                operation, (int)response.StatusCode, watch.ElapsedMilliseconds, attempt);

            if (response.IsSuccessStatusCode) return response;

            var status = response.StatusCode;
            response.Dispose();

            if (status == HttpStatusCode.BadGateway && attempt == 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(options.Value.RetryDelaySeconds), ct);
                continue;
            }
            if (status == HttpStatusCode.UnprocessableEntity) throw new OrchestratorRejectedException();
            throw new OrchestratorUnavailableException($"The orchestrator returned {(int)status}.");
        }
    }

    private sealed record RunRequest(
        [property: JsonPropertyName("task")] string Task,
        [property: JsonPropertyName("text")] string Text);

    private sealed class RunResponse
    {
        [JsonPropertyName("result")] public JsonElement Result { get; set; }
        [JsonPropertyName("seconds")] public double Seconds { get; set; }
        [JsonPropertyName("warnings")] public List<string>? Warnings { get; set; }
    }
}
