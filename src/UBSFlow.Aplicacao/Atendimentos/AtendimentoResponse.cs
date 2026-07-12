namespace UBSFlow.Aplicacao.Atendimentos;

public sealed record AtendimentoResponse(
    Guid Id,
    Guid CheckInId,
    Guid PacienteId,
    Guid ProfissionalId,
    string Queixa,
    string HipoteseDiagnostica,
    string Conduta,
    string? Prescricao,
    string? Encaminhamento,
    DateTimeOffset IniciadoEm,
    DateTimeOffset? FinalizadoEm);
