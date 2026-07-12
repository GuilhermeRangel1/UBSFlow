namespace UBSFlow.Aplicacao.Relatorios;

public sealed record AtendimentosPorPeriodoResponse(
    DateOnly Inicio,
    DateOnly Fim,
    int TotalAtendimentos,
    int TotalFinalizados);
