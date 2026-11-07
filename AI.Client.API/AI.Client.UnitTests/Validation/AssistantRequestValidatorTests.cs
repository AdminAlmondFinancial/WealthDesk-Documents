using AI.Client.API.Validation;

namespace AI.Client.UnitTests.Validation;

public class AssistantRequestValidatorTests
{
    private const string Style = "File note";

    [Fact]
    public void DocumentOverTwoMegabytes_ValidateDocument_Throws413()
    {
        var e = Assert.Throws<ValidationException>(() =>
            AssistantRequestValidator.ValidateDocument("a.docx", 2 * 1024 * 1024 + 1, Style, null));
        Assert.Equal(413, e.StatusCode);
    }

    [Fact]
    public void DocumentExactlyTwoMegabytes_ValidateDocument_Passes() =>
        AssistantRequestValidator.ValidateDocument("a.pdf", 2 * 1024 * 1024, Style, null);

    [Theory]
    [InlineData("a.exe")]
    [InlineData("a.doc")]
    [InlineData("a")]
    public void UnsupportedExtension_ValidateDocument_Throws415(string name)
    {
        var e = Assert.Throws<ValidationException>(() => AssistantRequestValidator.ValidateDocument(name, 10, Style, null));
        Assert.Equal(415, e.StatusCode);
    }

    [Fact]
    public void UnknownStyle_ValidateDocument_Throws() =>
        Assert.Throws<ValidationException>(() => AssistantRequestValidator.ValidateDocument("a.docx", 10, "Fancy", null));

    [Fact]
    public void EmptyText_ValidateChat_Throws() =>
        Assert.Throws<ValidationException>(() => AssistantRequestValidator.ValidateChat(" ", Style, "Phi-4"));

    [Fact]
    public void UnknownTimeZone_ResolveTimeZone_FallsBackToUtc() =>
        Assert.Equal(TimeZoneInfo.Utc, AssistantRequestValidator.ResolveTimeZone("Not/AZone"));
}
