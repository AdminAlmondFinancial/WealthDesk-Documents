namespace AI.Client.API.Assistant;

public interface IChatSession
{
    Guid Id { get; }
    string UserId { get; }
    string Title { get; }
    DateTime CreatedUtc { get; }
}
