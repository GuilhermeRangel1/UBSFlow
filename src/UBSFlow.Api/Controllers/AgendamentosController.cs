using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Comum;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Route("agendamentos")]
public class AgendamentosController : ControllerBase
{
    private readonly AgendamentoService agendamentoService;

    public AgendamentosController(AgendamentoService agendamentoService)
    {
        this.agendamentoService = agendamentoService;
    }

    [HttpGet]
    public IActionResult Listar(
        [FromQuery] Guid? pacienteId,
        [FromQuery] Guid? profissionalId,
        [FromQuery] DateOnly? data,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        try
        {
            var request = new ListarAgendamentosRequest(
                pacienteId,
                profissionalId,
                data,
                pagina,
                tamanhoPagina);

            return Ok(agendamentoService.Listar(request));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        var agendamento = agendamentoService.ObterPorId(id);

        return agendamento is null ? NotFound() : Ok(agendamento);
    }

    [HttpPost]
    public IActionResult Criar(CriarAgendamentoRequest request)
    {
        try
        {
            var agendamento = agendamentoService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = agendamento.Id }, agendamento);
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

    [HttpPatch("{id:guid}/remarcar")]
    public IActionResult Remarcar(Guid id, RemarcarAgendamentoRequest request)
    {
        try
        {
            return Ok(agendamentoService.Remarcar(id, request));
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
