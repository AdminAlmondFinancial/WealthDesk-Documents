namespace AI.Client.API.Validation;

public static class AssistantRequestValidator
{
    public const long MaxDocumentBytes = 2 * 1024 * 1024;
    public const int MaxTextLength = 10_000;

    public static readonly IReadOnlyList<string> HouseStyles = new[]
    {
        "Wealth Desk house style", "Suitability report", "Annual review pack", "Client letter", "File note",
    };

    private static readonly string[] AllowedExtensions = { ".docx", ".pdf" };

    public static void ValidateDocument(string? fileName, long length, string? style, string? instructions)
    {
        if (string.IsNullOrWhiteSpace(fileName) || length <= 0)
            throw new ValidationException("Attach a document to format.");
        if (length > MaxDocumentBytes)
            throw new ValidationException("The document is over the 2 MB limit.", 413);
        if (!AllowedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
            throw new ValidationException("Only Word or PDF documents can be formatted.", 415);
        ValidateStyle(style);
        if (instructions is { Length: > MaxTextLength })
            throw new ValidationException("The instructions are too long.");
    }

    public static void ValidateChat(string? text, string? style, string? model)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ValidationException("Message text is required.");
        if (text.Length > MaxTextLength) throw new ValidationException("The message is too long.");
        ValidateStyle(style);
        if (string.IsNullOrWhiteSpace(model) || model.Length > 50) throw new ValidationException("A model is required.");
    }

    public static TimeZoneInfo ResolveTimeZone(string? ianaId)
    {
        if (string.IsNullOrWhiteSpace(ianaId)) return TimeZoneInfo.Utc;
        try { return TimeZoneInfo.FindSystemTimeZoneById(ianaId); }
        catch (Exception e) when (e is TimeZoneNotFoundException or InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
    }

    private static void ValidateStyle(string? style)
    {
        if (style is null || !HouseStyles.Contains(style)) throw new ValidationException("Choose one of the house styles.");
    }
}
