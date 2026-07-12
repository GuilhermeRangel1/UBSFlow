using UBSFlow.Dominio.Agenda;

namespace UBSFlow.Aplicacao.Agenda;

public sealed record AgendamentoResponse(
    Guid Id,
    Guid PacienteId,
    Guid ProfissionalId,
    DateTimeOffset Inicio,
    DateTimeOffset Fim,
    StatusAgendamento Status);
