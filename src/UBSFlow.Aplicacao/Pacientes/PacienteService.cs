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

    public ResultadoPaginado<PacienteResponse> Listar(ListarPacientesRequest request)
    {
        ValidarPaginacao(request);

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

        var totalItens = pacientes.Count();
        var totalPaginas = (int)Math.Ceiling(totalItens / (double)request.TamanhoPagina);
        var itens = pacientes
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(MapearPaciente)
            .ToList();

        return new ResultadoPaginado<PacienteResponse>(
            itens,
            request.Pagina,
            request.TamanhoPagina,
            totalItens,
            totalPaginas);
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

    private static void ValidarPaginacao(ListarPacientesRequest request)
    {
        if (request.Pagina < 1)
        {
            throw new ValidacaoException("Pagina deve ser maior ou igual a 1.");
        }

        if (request.TamanhoPagina is < 1 or > 100)
        {
            throw new ValidacaoException("Tamanho da pagina deve estar entre 1 e 100.");
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
