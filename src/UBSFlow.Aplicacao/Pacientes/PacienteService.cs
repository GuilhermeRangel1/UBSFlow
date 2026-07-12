using UBSFlow.Aplicacao.Comum;
using UBSFlow.Dominio.Pacientes;

namespace UBSFlow.Aplicacao.Pacientes;

public class PacienteService
{
    private readonly IPacienteRepositorio pacienteRepositorio;

    public PacienteService(IPacienteRepositorio pacienteRepositorio)
    {
        this.pacienteRepositorio = pacienteRepositorio;
    }

    public IReadOnlyCollection<PacienteResponse> Listar(ListarPacientesRequest request)
    {
        var pacientes = pacienteRepositorio.Listar().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            pacientes = pacientes.Where(paciente =>
                paciente.Nome.Contains(request.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            pacientes = pacientes.Where(paciente => paciente.Cpf == request.Cpf);
        }

        return pacientes
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
        ValidarCriacao(request);

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

    private static void ValidarCriacao(CriarPacienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            throw new ValidacaoException("Nome e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Cpf))
        {
            throw new ValidacaoException("CPF e obrigatorio.");
        }

        if (request.Cpf.Length != 11 || request.Cpf.Any(c => !char.IsDigit(c)))
        {
            throw new ValidacaoException("CPF deve conter exatamente 11 digitos.");
        }

        if (request.DataNascimento > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ValidacaoException("Data de nascimento nao pode estar no futuro.");
        }

        if (string.IsNullOrWhiteSpace(request.Telefone))
        {
            throw new ValidacaoException("Telefone e obrigatorio.");
        }
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
