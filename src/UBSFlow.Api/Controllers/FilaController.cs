using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Route("fila")]
public class FilaController : ControllerBase
{
    private readonly FilaAtendimentoService filaAtendimentoService;

    public FilaController(FilaAtendimentoService filaAtendimentoService)
    {
        this.filaAtendimentoService = filaAtendimentoService;
    }

    [HttpPost("check-ins")]
    public IActionResult CriarCheckIn(CriarCheckInRequest request)
    {
        try
        {
            return Created(string.Empty, filaAtendimentoService.CriarCheckIn(request));
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

    [HttpGet("hoje")]
    public IActionResult ListarFilaHoje()
    {
        return Ok(filaAtendimentoService.ListarFila(new ListarFilaRequest(null)));
    }

    [HttpGet]
    public IActionResult ListarFila([FromQuery] DateOnly? data)
    {
        return Ok(filaAtendimentoService.ListarFila(new ListarFilaRequest(data)));
    }
}
