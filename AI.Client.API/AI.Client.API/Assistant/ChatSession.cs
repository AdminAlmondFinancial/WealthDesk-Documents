using AI.Client.API.Validation;

namespace AI.Client.API.Assistant;

public sealed class ChatSession : IChatSession
{
    // New item: the id is generated here.
    public ChatSession(string userId, string title, DateTime nowUtc)
        : this(Guid.NewGuid(), userId, title, nowUtc) { }

    // Existing item: the id must be supplied.
    public ChatSession(Guid id, string userId, string title, DateTime createdUtc)
    {
        if (id == Guid.Empty) throw new ValidationException("Chat id is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("Chat title is required.");
        Id = id;
        UserId = userId;
        Title = title.Length > 200 ? title[..200] : title;
        CreatedUtc = createdUtc;
    }

    public Guid Id { get; }
    public string UserId { get; }
    public string Title { get; }
    public DateTime CreatedUtc { get; }
}
