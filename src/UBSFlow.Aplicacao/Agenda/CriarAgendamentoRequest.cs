namespace UBSFlow.Aplicacao.Agenda;

public sealed record CriarAgendamentoRequest(
    Guid PacienteId,
    Guid ProfissionalId,
    DateTimeOffset Inicio,
    DateTimeOffset Fim);
