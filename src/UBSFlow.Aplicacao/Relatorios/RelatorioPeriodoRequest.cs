namespace UBSFlow.Aplicacao.Relatorios;

public sealed record RelatorioPeriodoRequest(
    DateOnly Inicio,
    DateOnly Fim);
