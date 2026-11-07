using AI.Client.API.Validation;

namespace AI.Client.API.Assistant;

public sealed class FormattedDocument : IFormattedDocument
{
    // New item: the id is generated here.
    public FormattedDocument(string userId, string name, string meta, byte[] content, DateTime nowUtc)
        : this(Guid.NewGuid(), userId, name, meta, content, nowUtc) { }

    // Existing item: the id must be supplied.
    public FormattedDocument(Guid id, string userId, string name, string meta, byte[] content, DateTime createdUtc)
    {
        if (id == Guid.Empty) throw new ValidationException("Document id is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Document name is required.");
        if (content is null || content.Length == 0) throw new ValidationException("Document content is required.");
        Id = id;
        UserId = userId;
        Name = name;
        Meta = meta ?? string.Empty;
        Content = content;
        CreatedUtc = createdUtc;
    }

    public Guid Id { get; }
    public string UserId { get; }
    public string Name { get; }
    public string Meta { get; }
    public byte[] Content { get; }
    public DateTime CreatedUtc { get; }
}
