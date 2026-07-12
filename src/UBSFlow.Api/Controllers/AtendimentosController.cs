using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "ADMIN,MEDICO")]
[Route("atendimentos")]
public class AtendimentosController : ControllerBase
{
    private readonly AtendimentoService atendimentoService;

    public AtendimentosController(AtendimentoService atendimentoService)
    {
        this.atendimentoService = atendimentoService;
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        var atendimento = atendimentoService.ObterPorId(id);

        return atendimento is null ? NotFound() : Ok(atendimento);
    }

    [HttpPost]
    public IActionResult Criar(CriarAtendimentoRequest request)
    {
        try
        {
            var atendimento = atendimentoService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = atendimento.Id }, atendimento);
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

    [HttpPatch("{id:guid}/finalizar")]
    public IActionResult Finalizar(Guid id, FinalizarAtendimentoRequest request)
    {
        try
        {
            return Ok(atendimentoService.Finalizar(id, request));
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
