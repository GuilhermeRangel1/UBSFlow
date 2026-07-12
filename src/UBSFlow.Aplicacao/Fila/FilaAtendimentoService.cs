using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Fila;

public class FilaAtendimentoService
{
    private readonly ICheckInAtendimentoRepositorio checkInRepositorio;
    private readonly IAgendamentoRepositorio agendamentoRepositorio;
    private readonly ITriagemRepositorio triagemRepositorio;

    public FilaAtendimentoService(
        ICheckInAtendimentoRepositorio checkInRepositorio,
        IAgendamentoRepositorio agendamentoRepositorio,
        ITriagemRepositorio triagemRepositorio)
    {
        this.checkInRepositorio = checkInRepositorio;
        this.agendamentoRepositorio = agendamentoRepositorio;
        this.triagemRepositorio = triagemRepositorio;
    }

    public CheckInAtendimentoResponse CriarCheckIn(CriarCheckInRequest request)
    {
        if (request.AgendamentoId == Guid.Empty)
        {
            throw new ValidacaoException("Agendamento e obrigatorio.");
        }

        var agendamento = agendamentoRepositorio.ObterPorId(request.AgendamentoId);

        if (agendamento is null)
        {
            throw new ValidacaoException("Agendamento informado nao existe.");
        }

        if (agendamento.Status == StatusAgendamento.Cancelado)
        {
            throw new InvalidOperationException("Nao e possivel fazer check-in de agendamento cancelado.");
        }

        if (checkInRepositorio.ObterPorAgendamentoId(request.AgendamentoId) is not null)
        {
            throw new InvalidOperationException("Check-in ja realizado para este agendamento.");
        }

        var checkIn = new CheckInAtendimento(
            agendamento.Id,
            agendamento.PacienteId,
            agendamento.ProfissionalId,
            request.RealizadoEm ?? DateTimeOffset.UtcNow);

        checkInRepositorio.Adicionar(checkIn);

        return MapearCheckIn(checkIn, null);
    }

    public IReadOnlyCollection<CheckInAtendimentoResponse> ListarFila(ListarFilaRequest request)
    {
        var data = request.Data ?? DateOnly.FromDateTime(DateTime.Today);

        var triagensPorCheckIn = triagemRepositorio
            .Listar()
            .ToDictionary(triagem => triagem.CheckInId);

        return checkInRepositorio
            .Listar()
            .Where(checkIn => DateOnly.FromDateTime(checkIn.RealizadoEm.LocalDateTime) == data)
            .Select(checkIn =>
            {
                triagensPorCheckIn.TryGetValue(checkIn.Id, out var triagem);

                return MapearCheckIn(checkIn, triagem?.ClassificacaoRisco);
            })
            .OrderBy(response => ObterPrioridadeStatus(response.Status))
            .ThenByDescending(response => response.ClassificacaoRisco ?? 0)
            .ThenBy(response => response.RealizadoEm)
            .ToList();
    }

    private static int ObterPrioridadeStatus(StatusFilaAtendimento status)
    {
        return status switch
        {
            StatusFilaAtendimento.AguardandoAtendimento => 0,
            StatusFilaAtendimento.AguardandoTriagem => 1,
            _ => 2
        };
    }

    private static CheckInAtendimentoResponse MapearCheckIn(
        CheckInAtendimento checkIn,
        ClassificacaoRisco? classificacaoRisco)
    {
        return new CheckInAtendimentoResponse(
            checkIn.Id,
            checkIn.AgendamentoId,
            checkIn.PacienteId,
            checkIn.ProfissionalId,
            checkIn.RealizadoEm,
            checkIn.Status,
            classificacaoRisco);
    }
}
