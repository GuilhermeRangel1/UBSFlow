using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Dominio.Auditoria;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Triagens;

public class TriagemService
{
    private readonly AuditoriaService? auditoriaService;
    private readonly ICheckInAtendimentoRepositorio checkInRepositorio;
    private readonly ITriagemRepositorio triagemRepositorio;

    public TriagemService(
        ITriagemRepositorio triagemRepositorio,
        ICheckInAtendimentoRepositorio checkInRepositorio,
        AuditoriaService? auditoriaService = null)
    {
        this.triagemRepositorio = triagemRepositorio;
        this.checkInRepositorio = checkInRepositorio;
        this.auditoriaService = auditoriaService;
    }

    public TriagemResponse Criar(CriarTriagemRequest request)
    {
        ValidarRequest(request);

        var checkIn = checkInRepositorio.ObterPorId(request.CheckInId);

        if (checkIn is null)
        {
            throw new ValidacaoException("Check-in informado nao existe.");
        }

        if (checkIn.Status != StatusFilaAtendimento.AguardandoTriagem)
        {
            throw new InvalidOperationException("Check-in nao esta aguardando triagem.");
        }

        if (triagemRepositorio.ObterPorCheckInId(request.CheckInId) is not null)
        {
            throw new InvalidOperationException("Triagem ja realizada para este check-in.");
        }

        checkIn.IniciarTriagem();
        var classificacaoCalculada = ClassificadorRiscoTriagem.Classificar(request);
        var classificacaoFinal = ClassificadorRiscoTriagem.ObterMaisGrave(
            request.ClassificacaoRisco,
            classificacaoCalculada);

        var triagem = new Triagem(
            checkIn.Id,
            checkIn.PacienteId,
            request.Temperatura,
            request.PressaoSistolica,
            request.PressaoDiastolica,
            request.FrequenciaCardiaca,
            request.Sintomas,
            classificacaoFinal,
            request.Observacoes,
            request.RealizadaEm ?? DateTimeOffset.UtcNow);

        triagemRepositorio.Adicionar(triagem);
        checkIn.ConcluirTriagem();
        auditoriaService?.Registrar(
            AcaoAuditoria.TriagemRealizada,
            "Triagem",
            triagem.Id,
            $"Triagem realizada com classificacao {triagem.ClassificacaoRisco}.");

        return MapearTriagem(triagem);
    }

    public TriagemResponse? ObterPorId(Guid id)
    {
        var triagem = triagemRepositorio.ObterPorId(id);

        return triagem is null ? null : MapearTriagem(triagem);
    }

    private static void ValidarRequest(CriarTriagemRequest request)
    {
        if (request.CheckInId == Guid.Empty)
        {
            throw new ValidacaoException("Check-in e obrigatorio.");
        }

        if (request.Temperatura < 30 || request.Temperatura > 45)
        {
            throw new ValidacaoException("Temperatura deve estar entre 30 e 45 graus.");
        }

        if (request.PressaoSistolica <= 0 || request.PressaoDiastolica <= 0)
        {
            throw new ValidacaoException("Pressao arterial deve ser maior que zero.");
        }

        if (request.FrequenciaCardiaca <= 0)
        {
            throw new ValidacaoException("Frequencia cardiaca deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Sintomas))
        {
            throw new ValidacaoException("Sintomas sao obrigatorios.");
        }

        if (!Enum.IsDefined(request.ClassificacaoRisco))
        {
            throw new ValidacaoException("Classificacao de risco invalida.");
        }
    }

    private static TriagemResponse MapearTriagem(Triagem triagem)
    {
        return new TriagemResponse(
            triagem.Id,
            triagem.CheckInId,
            triagem.PacienteId,
            triagem.Temperatura,
            triagem.PressaoSistolica,
            triagem.PressaoDiastolica,
            triagem.FrequenciaCardiaca,
            triagem.Sintomas,
            triagem.ClassificacaoRisco,
            triagem.Observacoes,
            triagem.RealizadaEm);
    }
}
