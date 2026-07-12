using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Fila;

namespace UBSFlow.Aplicacao.Fila;

public class FilaAtendimentoService
{
    private readonly ICheckInAtendimentoRepositorio checkInRepositorio;
    private readonly IAgendamentoRepositorio agendamentoRepositorio;

    public FilaAtendimentoService(
        ICheckInAtendimentoRepositorio checkInRepositorio,
        IAgendamentoRepositorio agendamentoRepositorio)
    {
        this.checkInRepositorio = checkInRepositorio;
        this.agendamentoRepositorio = agendamentoRepositorio;
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

        return MapearCheckIn(checkIn);
    }

    public IReadOnlyCollection<CheckInAtendimentoResponse> ListarFila(ListarFilaRequest request)
    {
        var data = request.Data ?? DateOnly.FromDateTime(DateTime.Today);

        return checkInRepositorio
            .Listar()
            .Where(checkIn => DateOnly.FromDateTime(checkIn.RealizadoEm.LocalDateTime) == data)
            .OrderBy(checkIn => checkIn.RealizadoEm)
            .Select(MapearCheckIn)
            .ToList();
    }

    private static CheckInAtendimentoResponse MapearCheckIn(CheckInAtendimento checkIn)
    {
        return new CheckInAtendimentoResponse(
            checkIn.Id,
            checkIn.AgendamentoId,
            checkIn.PacienteId,
            checkIn.ProfissionalId,
            checkIn.RealizadoEm,
            checkIn.Status);
    }
}
