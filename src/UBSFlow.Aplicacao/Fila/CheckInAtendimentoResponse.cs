using UBSFlow.Dominio.Fila;

namespace UBSFlow.Aplicacao.Fila;

public sealed record CheckInAtendimentoResponse(
    Guid Id,
    Guid AgendamentoId,
    Guid PacienteId,
    Guid ProfissionalId,
    DateTimeOffset RealizadoEm,
    StatusFilaAtendimento Status);
