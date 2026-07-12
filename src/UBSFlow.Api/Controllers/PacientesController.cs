using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Listar()
    {
        return Ok(pacienteService.Listar());
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
    }
}
