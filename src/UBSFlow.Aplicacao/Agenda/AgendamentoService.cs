using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Agenda;

namespace UBSFlow.Aplicacao.Agenda;

public class AgendamentoService
{
    private readonly IAgendamentoRepositorio agendamentoRepositorio;
    private readonly IPacienteRepositorio pacienteRepositorio;
    private readonly IProfissionalRepositorio profissionalRepositorio;

    public AgendamentoService(
        IAgendamentoRepositorio agendamentoRepositorio,
        IPacienteRepositorio pacienteRepositorio,
        IProfissionalRepositorio profissionalRepositorio)
    {
        this.agendamentoRepositorio = agendamentoRepositorio;
        this.pacienteRepositorio = pacienteRepositorio;
        this.profissionalRepositorio = profissionalRepositorio;
    }

    public ResultadoPaginado<AgendamentoResponse> Listar(ListarAgendamentosRequest request)
    {
        ValidarPaginacao(request);

        var agendamentos = agendamentoRepositorio.Listar().AsEnumerable();

        if (request.PacienteId is not null)
        {
            agendamentos = agendamentos.Where(agendamento => agendamento.PacienteId == request.PacienteId);
        }

        if (request.ProfissionalId is not null)
        {
            agendamentos = agendamentos.Where(agendamento => agendamento.ProfissionalId == request.ProfissionalId);
        }

        if (request.Data is not null)
        {
            agendamentos = agendamentos.Where(agendamento =>
                DateOnly.FromDateTime(agendamento.Inicio.LocalDateTime) == request.Data);
        }

        var totalItens = agendamentos.Count();
        var totalPaginas = (int)Math.Ceiling(totalItens / (double)request.TamanhoPagina);
        var itens = agendamentos
            .OrderBy(agendamento => agendamento.Inicio)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(MapearAgendamento)
            .ToList();

        return new ResultadoPaginado<AgendamentoResponse>(
            itens,
            request.Pagina,
            request.TamanhoPagina,
            totalItens,
            totalPaginas);
    }

    public AgendamentoResponse? ObterPorId(Guid id)
    {
        var agendamento = agendamentoRepositorio.ObterPorId(id);

        return agendamento is null ? null : MapearAgendamento(agendamento);
    }

    public AgendamentoResponse Criar(CriarAgendamentoRequest request)
    {
        ValidarCriacao(request);

        if (pacienteRepositorio.ObterPorId(request.PacienteId) is null)
        {
            throw new ValidacaoException("Paciente informado nao existe.");
        }

        if (profissionalRepositorio.ObterPorId(request.ProfissionalId) is null)
        {
            throw new ValidacaoException("Profissional informado nao existe.");
        }

        if (ExisteConflitoDeHorario(request))
        {
            throw new InvalidOperationException("Profissional ja possui agendamento neste horario.");
        }

        var agendamento = new Agendamento(
            request.PacienteId,
            request.ProfissionalId,
            request.Inicio,
            request.Fim);

        agendamentoRepositorio.Adicionar(agendamento);

        return MapearAgendamento(agendamento);
    }

    private bool ExisteConflitoDeHorario(CriarAgendamentoRequest request)
    {
        return agendamentoRepositorio.Listar().Any(agendamento =>
            agendamento.ProfissionalId == request.ProfissionalId &&
            agendamento.Status != StatusAgendamento.Cancelado &&
            request.Inicio < agendamento.Fim &&
            request.Fim > agendamento.Inicio);
    }

    private static void ValidarCriacao(CriarAgendamentoRequest request)
    {
        if (request.PacienteId == Guid.Empty)
        {
            throw new ValidacaoException("Paciente e obrigatorio.");
        }

        if (request.ProfissionalId == Guid.Empty)
        {
            throw new ValidacaoException("Profissional e obrigatorio.");
        }

        if (request.Fim <= request.Inicio)
        {
            throw new ValidacaoException("Horario final deve ser maior que o horario inicial.");
        }
    }

    private static void ValidarPaginacao(ListarAgendamentosRequest request)
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

    private static AgendamentoResponse MapearAgendamento(Agendamento agendamento)
    {
        return new AgendamentoResponse(
            agendamento.Id,
            agendamento.PacienteId,
            agendamento.ProfissionalId,
            agendamento.Inicio,
            agendamento.Fim,
            agendamento.Status);
    }
}
