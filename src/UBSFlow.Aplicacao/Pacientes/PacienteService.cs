using UBSFlow.Dominio.Pacientes;

namespace UBSFlow.Aplicacao.Pacientes;

public class PacienteService
{
    private readonly IPacienteRepositorio pacienteRepositorio;

    public PacienteService(IPacienteRepositorio pacienteRepositorio)
    {
        this.pacienteRepositorio = pacienteRepositorio;
    }

    public IReadOnlyCollection<PacienteResponse> Listar()
    {
        return pacienteRepositorio
            .Listar()
            .Select(MapearPaciente)
            .ToList();
    }

    public PacienteResponse? ObterPorId(Guid id)
    {
        var paciente = pacienteRepositorio.ObterPorId(id);

        return paciente is null ? null : MapearPaciente(paciente);
    }

    public PacienteResponse Criar(CriarPacienteRequest request)
    {
        if (pacienteRepositorio.ObterPorCpf(request.Cpf) is not null)
        {
            throw new InvalidOperationException("Ja existe um paciente cadastrado com este CPF.");
        }

        var paciente = new Paciente(
            request.Nome,
            request.Cpf,
            request.DataNascimento,
            request.Telefone,
            request.Cns);

        pacienteRepositorio.Adicionar(paciente);

        return MapearPaciente(paciente);
    }

    private static PacienteResponse MapearPaciente(Paciente paciente)
    {
        return new PacienteResponse(
            paciente.Id,
            paciente.Nome,
            paciente.Cpf,
            paciente.Cns,
            paciente.DataNascimento,
            paciente.Telefone);
    }
}
