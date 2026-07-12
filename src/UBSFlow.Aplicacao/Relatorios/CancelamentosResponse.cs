namespace UBSFlow.Aplicacao.Relatorios;

public sealed record CancelamentosResponse(
    DateOnly Inicio,
    DateOnly Fim,
    int TotalCancelamentos);
