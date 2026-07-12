using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Auditoria;
using UBSFlow.Dominio.Agenda;

namespace UBSFlow.Aplicacao.Agenda;

public class AgendamentoService
{
    private readonly IAgendamentoRepositorio agendamentoRepositorio;
    private readonly AuditoriaService? auditoriaService;
    private readonly IPacienteRepositorio pacienteRepositorio;
    private readonly IProfissionalRepositorio profissionalRepositorio;

    public AgendamentoService(
        IAgendamentoRepositorio agendamentoRepositorio,
        IPacienteRepositorio pacienteRepositorio,
        IProfissionalRepositorio profissionalRepositorio,
        AuditoriaService? auditoriaService = null)
    {
        this.agendamentoRepositorio = agendamentoRepositorio;
        this.pacienteRepositorio = pacienteRepositorio;
        this.profissionalRepositorio = profissionalRepositorio;
        this.auditoriaService = auditoriaService;
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

    public AgendamentoResponse Remarcar(Guid id, RemarcarAgendamentoRequest request)
    {
        ValidarPeriodo(request.Inicio, request.Fim);

        var agendamento = agendamentoRepositorio.ObterPorId(id);

        if (agendamento is null)
        {
            throw new ValidacaoException("Agendamento informado nao existe.");
        }

        if (agendamento.Status == StatusAgendamento.Cancelado)
        {
            throw new InvalidOperationException("Agendamento cancelado nao pode ser remarcado.");
        }

        if (ExisteConflitoDeHorario(
            agendamento.ProfissionalId,
            request.Inicio,
            request.Fim,
            agendamento.Id))
        {
            throw new InvalidOperationException("Profissional ja possui agendamento neste horario.");
        }

        agendamento.Remarcar(request.Inicio, request.Fim);
        agendamentoRepositorio.SalvarAlteracoes();

        return MapearAgendamento(agendamento);
    }

    public AgendamentoResponse Cancelar(Guid id, CancelarAgendamentoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ValidacaoException("Motivo do cancelamento e obrigatorio.");
        }

        var agendamento = agendamentoRepositorio.ObterPorId(id);

        if (agendamento is null)
        {
            throw new ValidacaoException("Agendamento informado nao existe.");
        }

        if (agendamento.Status == StatusAgendamento.Cancelado)
        {
            throw new InvalidOperationException("Agendamento ja esta cancelado.");
        }

        agendamento.Cancelar(request.Motivo);
        agendamentoRepositorio.SalvarAlteracoes();
        auditoriaService?.Registrar(
            AcaoAuditoria.AgendamentoCancelado,
            "Agendamento",
            agendamento.Id,
            $"Agendamento cancelado. Motivo: {request.Motivo}");

        return MapearAgendamento(agendamento);
    }


    private bool ExisteConflitoDeHorario(CriarAgendamentoRequest request)
    {
        return ExisteConflitoDeHorario(
            request.ProfissionalId,
            request.Inicio,
            request.Fim,
            null);
    }

    private bool ExisteConflitoDeHorario(
        Guid profissionalId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        Guid? agendamentoIgnoradoId)
    {
        return agendamentoRepositorio.Listar().Any(agendamento =>
            agendamento.Id != agendamentoIgnoradoId &&
            agendamento.ProfissionalId == profissionalId &&
            agendamento.Status != StatusAgendamento.Cancelado &&
            inicio < agendamento.Fim &&
            fim > agendamento.Inicio);
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

        ValidarPeriodo(request.Inicio, request.Fim);
    }

    private static void ValidarPeriodo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        if (fim <= inicio)
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
            agendamento.Status,
            agendamento.MotivoCancelamento);
    }
}
