using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "ADMIN,RECEPCIONISTA,GESTOR")]
[Route("profissionais")]
public class ProfissionaisController : ControllerBase
{
    private readonly ProfissionalService profissionalService;

    public ProfissionaisController(ProfissionalService profissionalService)
    {
        this.profissionalService = profissionalService;
    }

    [HttpGet]
    public IActionResult Listar(
        [FromQuery] string? nome,
        [FromQuery] PapelProfissional? papel,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        try
        {
            var request = new ListarProfissionaisRequest(nome, papel, pagina, tamanhoPagina);

            return Ok(profissionalService.Listar(request));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        var profissional = profissionalService.ObterPorId(id);

        return profissional is null ? NotFound() : Ok(profissional);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public IActionResult Criar(CriarProfissionalRequest request)
    {
        try
        {
            var profissional = profissionalService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = profissional.Id }, profissional);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { mensagem = exception.Message });
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }
}
