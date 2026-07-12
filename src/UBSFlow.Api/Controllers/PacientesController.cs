using Microsoft.AspNetCore.Mvc;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Pacientes;

namespace UBSFlow.Api.Controllers;

[ApiController]
[Route("pacientes")]
public class PacientesController : ControllerBase
{
    private readonly PacienteService pacienteService;

    public PacientesController(PacienteService pacienteService)
    {
        this.pacienteService = pacienteService;
    }

    [HttpGet]
    public IActionResult Listar(
        [FromQuery] string? nome,
        [FromQuery] string? cpf,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        try
        {
            var request = new ListarPacientesRequest(nome, cpf, pagina, tamanhoPagina);

            return Ok(pacienteService.Listar(request));
        }
        catch (ValidacaoException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        var paciente = pacienteService.ObterPorId(id);

        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost]
    public IActionResult Criar(CriarPacienteRequest request)
    {
        try
        {
            var paciente = pacienteService.Criar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = paciente.Id }, paciente);
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
