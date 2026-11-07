using AI.Client.API.Assistant;
using AI.Client.API.Validation;

namespace AI.Client.UnitTests.Assistant;

public class EntityTests
{
    private static readonly DateTime Now = new(2026, 11, 7, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void NewItem_ChatSessionConstructor_GeneratesId() =>
        Assert.NotEqual(Guid.Empty, new ChatSession("u1", "Title", Now).Id);

    [Fact]
    public void EmptyId_ExistingChatSessionConstructor_Throws() =>
        Assert.Throws<ValidationException>(() => new ChatSession(Guid.Empty, "u1", "Title", Now));

    [Fact]
    public void BlankTitle_ChatSessionConstructor_Throws() =>
        Assert.Throws<ValidationException>(() => new ChatSession("u1", " ", Now));

    [Fact]
    public void EmptyContent_FormattedDocumentConstructor_Throws() =>
        Assert.Throws<ValidationException>(() => new FormattedDocument("u1", "a.docx", "meta", [], Now));

    [Fact]
    public void EmptyId_ExistingFormattedDocumentConstructor_Throws() =>
        Assert.Throws<ValidationException>(() => new FormattedDocument(Guid.Empty, "u1", "a.docx", "meta", [1], Now));
}
