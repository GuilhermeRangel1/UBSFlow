using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Dominio.Auditoria;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Fila;

namespace UBSFlow.Aplicacao.Atendimentos;

public class AtendimentoService
{
    private readonly AuditoriaService? auditoriaService;
    private readonly IAtendimentoRepositorio atendimentoRepositorio;
    private readonly ICheckInAtendimentoRepositorio checkInRepositorio;

    public AtendimentoService(
        IAtendimentoRepositorio atendimentoRepositorio,
        ICheckInAtendimentoRepositorio checkInRepositorio,
        AuditoriaService? auditoriaService = null)
    {
        this.atendimentoRepositorio = atendimentoRepositorio;
        this.checkInRepositorio = checkInRepositorio;
        this.auditoriaService = auditoriaService;
    }

    public AtendimentoResponse Criar(CriarAtendimentoRequest request)
    {
        ValidarRequest(request);

        var checkIn = checkInRepositorio.ObterPorId(request.CheckInId);

        if (checkIn is null)
        {
            throw new ValidacaoException("Check-in informado nao existe.");
        }

        if (checkIn.Status != StatusFilaAtendimento.AguardandoAtendimento)
        {
            throw new InvalidOperationException("Check-in nao esta aguardando atendimento medico.");
        }

        if (atendimentoRepositorio.ObterPorCheckInId(request.CheckInId) is not null)
        {
            throw new InvalidOperationException("Atendimento ja iniciado para este check-in.");
        }

        checkIn.IniciarAtendimento();

        var atendimento = new Atendimento(
            checkIn.Id,
            checkIn.PacienteId,
            checkIn.ProfissionalId,
            request.Queixa,
            request.HipoteseDiagnostica,
            request.Conduta,
            request.Prescricao,
            request.Encaminhamento,
            request.IniciadoEm ?? DateTimeOffset.UtcNow);

        atendimentoRepositorio.Adicionar(atendimento);

        return MapearAtendimento(atendimento);
    }

    public AtendimentoResponse? ObterPorId(Guid id)
    {
        var atendimento = atendimentoRepositorio.ObterPorId(id);

        return atendimento is null ? null : MapearAtendimento(atendimento);
    }

    public AtendimentoResponse Finalizar(Guid id, FinalizarAtendimentoRequest request)
    {
        var atendimento = atendimentoRepositorio.ObterPorId(id);

        if (atendimento is null)
        {
            throw new ValidacaoException("Atendimento informado nao existe.");
        }

        if (atendimento.FinalizadoEm is not null)
        {
            throw new InvalidOperationException("Atendimento ja esta finalizado.");
        }

        var checkIn = checkInRepositorio.ObterPorId(atendimento.CheckInId);

        if (checkIn is null)
        {
            throw new ValidacaoException("Check-in do atendimento nao existe.");
        }

        var finalizadoEm = request.FinalizadoEm ?? DateTimeOffset.UtcNow;

        if (finalizadoEm < atendimento.IniciadoEm)
        {
            throw new ValidacaoException("Horario de finalizacao nao pode ser anterior ao inicio do atendimento.");
        }

        atendimento.Finalizar(finalizadoEm);
        checkIn.FinalizarAtendimento();
        auditoriaService?.Registrar(
            AcaoAuditoria.AtendimentoFinalizado,
            "Atendimento",
            atendimento.Id,
            "Atendimento finalizado.");

        return MapearAtendimento(atendimento);
    }

    private static void ValidarRequest(CriarAtendimentoRequest request)
    {
        if (request.CheckInId == Guid.Empty)
        {
            throw new ValidacaoException("Check-in e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Queixa))
        {
            throw new ValidacaoException("Queixa e obrigatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.HipoteseDiagnostica))
        {
            throw new ValidacaoException("Hipotese diagnostica e obrigatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.Conduta))
        {
            throw new ValidacaoException("Conduta e obrigatoria.");
        }
    }

    private static AtendimentoResponse MapearAtendimento(Atendimento atendimento)
    {
        return new AtendimentoResponse(
            atendimento.Id,
            atendimento.CheckInId,
            atendimento.PacienteId,
            atendimento.ProfissionalId,
            atendimento.Queixa,
            atendimento.HipoteseDiagnostica,
            atendimento.Conduta,
            atendimento.Prescricao,
            atendimento.Encaminhamento,
            atendimento.IniciadoEm,
            atendimento.FinalizadoEm);
    }
}
