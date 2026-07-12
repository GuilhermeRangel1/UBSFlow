namespace UBSFlow.Aplicacao.Atendimentos;

public sealed record CriarAtendimentoRequest(
    Guid CheckInId,
    string Queixa,
    string HipoteseDiagnostica,
    string Conduta,
    string? Prescricao,
    string? Encaminhamento,
    DateTimeOffset? IniciadoEm = null);
