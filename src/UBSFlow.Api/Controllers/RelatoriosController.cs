using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Relatorios;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Route("relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly RelatorioService relatorioService;

    public RelatoriosController(RelatorioService relatorioService)
    {
        this.relatorioService = relatorioService;
    }

    [HttpGet("atendimentos")]
    public IActionResult ObterAtendimentosPorPeriodo([FromQuery] DateOnly inicio, [FromQuery] DateOnly fim)
    {
        try
        {
            return Ok(relatorioService.ObterAtendimentosPorPeriodo(new RelatorioPeriodoRequest(inicio, fim)));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    [HttpGet("classificacoes-risco")]
    public IActionResult ObterClassificacoesRisco([FromQuery] DateOnly inicio, [FromQuery] DateOnly fim)
    {
        try
        {
            return Ok(relatorioService.ObterClassificacoesRisco(new RelatorioPeriodoRequest(inicio, fim)));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    [HttpGet("cancelamentos")]
    public IActionResult ObterCancelamentos([FromQuery] DateOnly inicio, [FromQuery] DateOnly fim)
    {
        try
        {
            return Ok(relatorioService.ObterCancelamentos(new RelatorioPeriodoRequest(inicio, fim)));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }
}
