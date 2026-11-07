namespace AI.Client.API.Assistant;

public interface IFormattedDocument
{
    Guid Id { get; }
    string UserId { get; }
    string Name { get; }
    string Meta { get; }
    byte[] Content { get; }
    DateTime CreatedUtc { get; }
}
