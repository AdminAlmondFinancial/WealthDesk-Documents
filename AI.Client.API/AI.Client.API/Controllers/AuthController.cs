using AI.Client.API.Dto;
using AI.Client.API.UserManagerFactory;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.Client.API.Controllers;

[ApiController]
[Authorize]
[Route("api/auth")]
public sealed class AuthController(IUserManagerFactory users, IMapper mapper) : ControllerBase
{
    // The Entra token is validated by the bearer handler. Every user is granted all access areas.
    [HttpPost("login")]
    public ActionResult<LoginResultDto> Login() => mapper.Map<LoginResultDto>(users.Create(User));
}
