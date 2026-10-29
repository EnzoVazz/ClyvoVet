using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
[Tags("Autenticação")]
public class AuthController : ControllerBase
{
    private readonly Auth.AuthService _authService;

    public AuthController(Auth.AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Emite um token JWT",
        Description = "Valida usuário e senha de demonstração e devolve um Bearer token com a role (Admin, Veterinario ou Recepcionista)."
    )]
    [ProducesResponseType(typeof(Auth.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Auth.LoginResponse> Login([FromBody] Auth.LoginRequest request)
    {
        return Ok(_authService.Autenticar(request));
    }
}
