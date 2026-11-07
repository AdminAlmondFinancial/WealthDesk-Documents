using AI.Client.API.UserManagerFactory;

namespace AI.Client.API.Assistant.Manager;

public sealed record FormatResult(IFormattedDocument Document, IReadOnlyList<string> Changes, IReadOnlyList<string> Warnings);

public interface IAssistantManager
{
    Task<IReadOnlyList<ChatGroup>> GetHistoryAsync(CurrentUser user, string? timeZone, CancellationToken ct);

    Task<FormatResult> FormatDocumentAsync(
        CurrentUser user, string fileName, byte[] content, string? style, string? instructions, CancellationToken ct);

    Task<IFormattedDocument> GetDocumentAsync(CurrentUser user, Guid id, CancellationToken ct);

    Task<string> ChatAsync(CurrentUser user, string? text, string? style, string? model, CancellationToken ct);
}
