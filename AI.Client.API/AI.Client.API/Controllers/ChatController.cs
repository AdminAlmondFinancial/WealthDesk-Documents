using AI.Client.API.Assistant.Manager;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/chat")]
public sealed class ChatController(IUserManagerFactory users, IAssistantManager manager) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatResultDto>> Post(ChatRequestDto request, CancellationToken ct)
    {
        var text = await manager.ChatAsync(users.Create(User), request.Text, request.Style, request.Model, ct);
        return Ok(new ChatResultDto(text));
    }
}
