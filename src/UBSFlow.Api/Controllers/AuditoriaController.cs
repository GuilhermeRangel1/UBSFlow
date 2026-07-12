using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Auditoria;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("auditoria")]
public class AuditoriaController : ControllerBase
{
    private readonly AuditoriaService auditoriaService;

    public AuditoriaController(AuditoriaService auditoriaService)
    {
        this.auditoriaService = auditoriaService;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(auditoriaService.Listar());
    }
}
