namespace UBSFlow.Aplicacao.Agenda;

public sealed record RemarcarAgendamentoRequest(
    DateTimeOffset Inicio,
    DateTimeOffset Fim);
