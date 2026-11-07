using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController(IUserManagerFactory users, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public ActionResult<MeDto> Get() => mapper.Map<MeDto>(users.Create(User));
}
