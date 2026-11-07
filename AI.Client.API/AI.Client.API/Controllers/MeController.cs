using AI.Client.API.Automapper;
using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController(IUserManagerFactory users, AssistantMapper mapper) : ControllerBase
{
    [HttpGet]
    public ActionResult<MeDto> Get() => mapper.ToMeDto(users.Create(User));
}
