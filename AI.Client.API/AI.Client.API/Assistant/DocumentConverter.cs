using AI.Client.API.Validation;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;

namespace AI.Client.API.Assistant;

public sealed class DocumentConverter : IDocumentConverter
{
    public string ExtractText(byte[] content, string fileName)
    {
        try
        {
            using var stream = new MemoryStream(content);
            return Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".docx" => ExtractDocx(stream),
                ".pdf" => ExtractPdf(stream),
                _ => throw new ValidationException("Only Word or PDF documents can be formatted.", 415),
            };
        }
        catch (Exception e) when (e is OpenXmlPackageException or InvalidDataException or IOException
                                      or UglyToad.PdfPig.Core.PdfDocumentFormatException)
        {
            throw new ValidationException("The document could not be read.");
        }
    }

    public byte[] BuildDocx(string text)
    {
        using var stream = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var main = doc.AddMainDocumentPart();
            var body = new Body();
            foreach (var line in text.Replace("\r\n", "\n").Split('\n'))
                body.Append(new Paragraph(new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve })));
            main.Document = new Document(body);
            main.Document.Save();
        }
        return stream.ToArray();
    }

    private static string ExtractDocx(Stream stream)
    {
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart?.Document?.Body
                   ?? throw new ValidationException("The document could not be read.");
        return string.Join("\n", body.Descendants<Paragraph>().Select(p => p.InnerText));
    }

    private static string ExtractPdf(Stream stream)
    {
        using var pdf = PdfDocument.Open(stream);
        return string.Join("\n", pdf.GetPages().Select(p => p.Text));
    }
}
