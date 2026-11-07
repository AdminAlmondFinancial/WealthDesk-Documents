namespace AI.Client.API.Assistant;

// The orchestrator is reached through the wealthdesk-documents Worker (POST /api/ai/run).
// The request/response will change again with the /fill endpoint (template + flagged fields).
// Keep every shape that touches the orchestrator in this file and in OrchestratorDataStore.

public sealed record FormatRequest(string Text);

/// <summary>Formatted text plus the warnings the orchestrator raised. Warnings are shown to a person, never acted on.</summary>
public sealed record FormatOutcome(string FormattedText, double Seconds, IReadOnlyList<string> Warnings);

public sealed class OrchestratorRejectedException() : Exception("The orchestrator could not process the request.");

public sealed class OrchestratorUnavailableException(string message) : Exception(message);

public sealed class OrchestratorTimeoutException() : Exception("The orchestrator took too long to reply.");

public sealed record ChatGroup(string Label, IReadOnlyList<IChatSession> Items);
