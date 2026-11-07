using AI.Client.API.AccountUsers;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.UserManagerFactory;
using AI.Client.API.Validation;

namespace AI.Client.API.Assistant.Manager;

public sealed class AssistantManager(
    IChatDataStore chats,
    IDocumentDataStore documents,
    IOrchestratorDataStore orchestrator,
    IDocumentConverter converter,
    TimeProvider time) : IAssistantManager
{
    public async Task<IReadOnlyList<ChatGroup>> GetHistoryAsync(CurrentUser user, string? timeZone, CancellationToken ct)
    {
        user.Require(AccessAreas.History);
        var zone = AssistantRequestValidator.ResolveTimeZone(timeZone);
        var today = TimeZoneInfo.ConvertTime(time.GetUtcNow(), zone).Date;

        var items = await chats.ListForUserAsync(user.Id, ct);
        return items
            .GroupBy(c => Label(TimeZoneInfo.ConvertTimeFromUtc(c.CreatedUtc, zone).Date, today))
            .Select(g => new ChatGroup(g.Key, g.ToList()))
            .OrderBy(g => Order(g.Label))
            .ToList();
    }

    public async Task<FormatResult> FormatDocumentAsync(
        CurrentUser user, string fileName, byte[] content, string? style, string? instructions, CancellationToken ct)
    {
        user.Require(AccessAreas.Format);
        AssistantRequestValidator.ValidateDocument(fileName, content.Length, style, instructions);

        var text = converter.ExtractText(content, fileName);
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException("The document has no text to format.");
        if (System.Text.Encoding.UTF8.GetByteCount(text) > AssistantRequestValidator.MaxDocumentBytes)
            throw new ValidationException("The document text is over the 2 MB limit.", 413);

        // Provisional: the orchestrator's format task takes text only. Style and instructions are
        // validated but not sent until the orchestrator supports them (see /fill in the handoff).
        var outcome = await orchestrator.FormatAsync(new FormatRequest(text), ct);

        var now = time.GetUtcNow().UtcDateTime;
        var outName = $"{Path.GetFileNameWithoutExtension(fileName)} (formatted).docx";
        var document = new FormattedDocument(
            user.Id, outName, $"Word document, formatted to the {style}", converter.BuildDocx(outcome.FormattedText), now);
        await documents.AddAsync(document, ct);
        await chats.AddAsync(new ChatSession(user.Id, $"Formatted {Path.GetFileNameWithoutExtension(fileName)}", now), ct);

        return new FormatResult(document, ["Reformatted the document text"], outcome.Warnings);
    }

    public async Task<IFormattedDocument> GetDocumentAsync(CurrentUser user, Guid id, CancellationToken ct)
    {
        user.Require(AccessAreas.Format);
        return await documents.GetForUserAsync(id, user.Id, ct) ?? throw new NotFoundException();
    }

    public Task<string> ChatAsync(CurrentUser user, string? text, string? style, string? model, CancellationToken ct)
    {
        user.Require(AccessAreas.Chat);
        AssistantRequestValidator.ValidateChat(text, style, model);
        // The orchestrator has no chat task yet (only format and extract).
        throw new ValidationException("Chat is not available yet.", 501);
    }

    private static string Label(DateTime day, DateTime today) =>
        (today - day).Days switch { <= 0 => "Today", 1 => "Yesterday", <= 7 => "Previous 7 days", _ => "Older" };

    private static int Order(string label) =>
        label switch { "Today" => 0, "Yesterday" => 1, "Previous 7 days" => 2, _ => 3 };
}
