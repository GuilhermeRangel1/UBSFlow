using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Triagens;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("triagens")]
public class TriagensController : ControllerBase
{
    private readonly TriagemService triagemService;

    public TriagensController(TriagemService triagemService)
    {
        this.triagemService = triagemService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ADMIN,ENFERMEIRO,MEDICO")]
    public IActionResult ObterPorId(Guid id)
    {
        var triagem = triagemService.ObterPorId(id);

        return triagem is null ? NotFound() : Ok(triagem);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,ENFERMEIRO")]
    public IActionResult Criar(CriarTriagemRequest request)
    {
        try
        {
            var triagem = triagemService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = triagem.Id }, triagem);
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
