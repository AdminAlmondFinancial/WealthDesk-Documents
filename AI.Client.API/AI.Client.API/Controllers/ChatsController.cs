using AI.Client.API.Assistant.Manager;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/chats")]
public sealed class ChatsController(IUserManagerFactory users, IAssistantManager manager, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HistoryGroupDto>>> Get(CancellationToken ct)
    {
        var groups = await manager.GetHistoryAsync(users.Create(User), Request.Headers["X-Timezone"], ct);
        return Ok(mapper.Map<List<HistoryGroupDto>>(groups));
    }
}
