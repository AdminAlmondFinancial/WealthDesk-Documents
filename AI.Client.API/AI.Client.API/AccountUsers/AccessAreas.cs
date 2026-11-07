namespace AI.Client.API.AccountUsers;

public static class AccessAreas
{
    public const string Chat = "Assistant.Chat";
    public const string Format = "Assistant.Format";
    public const string History = "Assistant.History";

    // Every area must be listed here. Every user is granted all of them on login.
    public static readonly IReadOnlyList<string> All = new[] { Chat, Format, History };
}
