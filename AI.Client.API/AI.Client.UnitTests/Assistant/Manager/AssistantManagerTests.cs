using AI.Client.API.AccountUsers;
using AI.Client.API.Assistant;
using AI.Client.API.Assistant.DataStores;
using AI.Client.API.Assistant.Manager;
using AI.Client.API.UserManagerFactory;
using AI.Client.API.Validation;

namespace AI.Client.UnitTests.Assistant.Manager;

public class AssistantManagerTests
{
    private static readonly CurrentUser User = new("u1", "Sarah", "Paraplanner", AccessAreas.All);
    private static readonly CurrentUser NoAccess = new("u2", "Nobody", "None", []);

    [Fact]
    public async Task UserWithoutFormatArea_FormatDocumentAsync_ThrowsForbidden()
    {
        var (manager, _, _) = Create();
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            manager.FormatDocumentAsync(NoAccess, "a.docx", [1], "File note", null, CancellationToken.None));
    }

    [Fact]
    public async Task OversizedDocument_FormatDocumentAsync_NeverCallsOrchestrator()
    {
        var (manager, orchestrator, _) = Create();
        var big = new byte[AssistantRequestValidator.MaxDocumentBytes + 1];
        var e = await Assert.ThrowsAsync<ValidationException>(() =>
            manager.FormatDocumentAsync(User, "a.docx", big, "File note", null, CancellationToken.None));
        Assert.Equal(413, e.StatusCode);
        Assert.Equal(0, orchestrator.Calls);
    }

    [Fact]
    public async Task ValidDocument_FormatDocumentAsync_ReturnsWarningsAndStoresOutput()
    {
        var (manager, orchestrator, documents) = Create();
        var result = await manager.FormatDocumentAsync(User, "Letter.docx", [1, 2], "Client letter", null, CancellationToken.None);
        Assert.Equal("Letter (formatted).docx", result.Document.Name);
        Assert.Equal(["check totals"], result.Warnings);
        Assert.Equal(1, orchestrator.Calls);
        Assert.Single(documents.Items);
    }

    [Fact]
    public async Task OtherUsersDocument_GetDocumentAsync_ThrowsNotFound()
    {
        var (manager, _, _) = Create();
        var result = await manager.FormatDocumentAsync(User, "a.docx", [1], "File note", null, CancellationToken.None);
        var other = new CurrentUser("someone-else", "X", "Y", AccessAreas.All);
        await Assert.ThrowsAsync<NotFoundException>(() => manager.GetDocumentAsync(other, result.Document.Id, CancellationToken.None));
    }

    [Fact]
    public async Task AnyRequest_ChatAsync_Returns501UntilOrchestratorSupportsChat()
    {
        var (manager, _, _) = Create();
        var e = await Assert.ThrowsAsync<ValidationException>(() => manager.ChatAsync(User, "hi", "File note", "Phi-4", CancellationToken.None));
        Assert.Equal(501, e.StatusCode);
    }

    private static (AssistantManager, FakeOrchestrator, FakeDocuments) Create()
    {
        var orchestrator = new FakeOrchestrator();
        var documents = new FakeDocuments();
        return (new AssistantManager(new FakeChats(), documents, orchestrator, new FakeConverter(), TimeProvider.System), orchestrator, documents);
    }

    private sealed class FakeOrchestrator : IOrchestratorDataStore
    {
        public int Calls { get; private set; }
        public Task<FormatOutcome> FormatAsync(FormatRequest request, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(new FormatOutcome("Formatted", 1, ["check totals"]));
        }
    }

    private sealed class FakeConverter : IDocumentConverter
    {
        public string ExtractText(byte[] content, string fileName) => "text";
        public byte[] BuildDocx(string text) => [9];
    }

    private sealed class FakeChats : IChatDataStore
    {
        private readonly List<IChatSession> _items = [];
        public Task<IReadOnlyList<IChatSession>> ListForUserAsync(string userId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<IChatSession>>(_items.Where(c => c.UserId == userId).ToList());
        public Task AddAsync(IChatSession chat, CancellationToken ct) { _items.Add(chat); return Task.CompletedTask; }
    }

    private sealed class FakeDocuments : IDocumentDataStore
    {
        public List<IFormattedDocument> Items { get; } = [];
        public Task AddAsync(IFormattedDocument document, CancellationToken ct) { Items.Add(document); return Task.CompletedTask; }
        public Task<IFormattedDocument?> GetForUserAsync(Guid id, string userId, CancellationToken ct) =>
            Task.FromResult(Items.FirstOrDefault(d => d.Id == id && d.UserId == userId));
    }
}
