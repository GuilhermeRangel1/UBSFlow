namespace UBSFlow.Aplicacao.Fila;

public sealed record CriarCheckInRequest(
    Guid AgendamentoId,
    DateTimeOffset? RealizadoEm = null);
