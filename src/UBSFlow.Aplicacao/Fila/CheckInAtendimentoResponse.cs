using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Fila;

public sealed record CheckInAtendimentoResponse(
    Guid Id,
    Guid AgendamentoId,
    Guid PacienteId,
    Guid ProfissionalId,
    DateTimeOffset RealizadoEm,
    StatusFilaAtendimento Status,
    ClassificacaoRisco? ClassificacaoRisco);
