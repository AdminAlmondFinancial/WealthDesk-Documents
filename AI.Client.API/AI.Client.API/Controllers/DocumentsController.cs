using AI.Client.API.Assistant.Manager;
using AI.Client.API.Automapper;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/documents")]
public sealed class DocumentsController(IUserManagerFactory users, IAssistantManager manager, AssistantMapper mapper) : ControllerBase
{
    private const string DocxContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    [HttpPost("format")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<ActionResult<FormatResultDto>> Format(
        IFormFile? file, [FromForm] string? style, [FromForm] string? instructions, CancellationToken ct)
    {
        var user = users.Create(User);
        if (file is null) return BadRequest(new { error = "Attach a document to format." });

        // Size is checked by the manager before anything is sent to the orchestrator.
        using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer, ct);

        var result = await manager.FormatDocumentAsync(user, file.FileName, buffer.ToArray(), style, instructions, ct);
        return Ok(mapper.ToFormatResultDto(result));
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var document = await manager.GetDocumentAsync(users.Create(User), id, ct);
        return File(document.Content, DocxContentType, document.Name);
    }
}
