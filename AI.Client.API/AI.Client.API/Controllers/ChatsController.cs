using AI.Client.API.Assistant.Manager;
using AI.Client.API.Automapper;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/chats")]
public sealed class ChatsController(IUserManagerFactory users, IAssistantManager manager, AssistantMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HistoryGroupDto>>> Get(CancellationToken ct)
    {
        var groups = await manager.GetHistoryAsync(users.Create(User), Request.Headers["X-Timezone"], ct);
        return Ok(groups.Select(mapper.ToHistoryGroupDto).ToList());
    }
}
