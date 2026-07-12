using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBSFlow.Api.Autenticacao;
using UBSFlow.Aplicacao.Autenticacao;
using UBSFlow.Aplicacao.Comum;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService authService;
    private readonly TokenService tokenService;

    public AuthController(AuthService authService, TokenService tokenService)
    {
        this.authService = authService;
        this.tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        try
        {
            var usuario = authService.Autenticar(request);

            return Ok(tokenService.GerarToken(usuario));
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new { mensagem = exception.Message });
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }
}
