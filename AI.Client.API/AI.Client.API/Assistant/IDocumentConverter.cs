namespace AI.Client.API.Assistant;

public interface IDocumentConverter
{
    /// <summary>Extracts plain text from a .docx or .pdf. The orchestrator takes text, not files.</summary>
    string ExtractText(byte[] content, string fileName);

    /// <summary>Builds a .docx with one paragraph per line of text.</summary>
    byte[] BuildDocx(string text);
}
