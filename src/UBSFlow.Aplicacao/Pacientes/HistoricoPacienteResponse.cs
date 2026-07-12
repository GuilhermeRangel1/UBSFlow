using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Pacientes;

public sealed record HistoricoPacienteResponse(
    Guid PacienteId,
    IReadOnlyCollection<AgendamentoHistoricoResponse> Agendamentos,
    IReadOnlyCollection<TriagemHistoricoResponse> Triagens,
    IReadOnlyCollection<AtendimentoHistoricoResponse> Atendimentos);

public sealed record AgendamentoHistoricoResponse(
    Guid Id,
    Guid ProfissionalId,
    DateTimeOffset Inicio,
    DateTimeOffset Fim,
    StatusAgendamento Status);

public sealed record TriagemHistoricoResponse(
    Guid Id,
    Guid CheckInId,
    decimal Temperatura,
    int PressaoSistolica,
    int PressaoDiastolica,
    int FrequenciaCardiaca,
    string Sintomas,
    ClassificacaoRisco ClassificacaoRisco,
    DateTimeOffset RealizadaEm);

public sealed record AtendimentoHistoricoResponse(
    Guid Id,
    Guid CheckInId,
    Guid ProfissionalId,
    string Queixa,
    string HipoteseDiagnostica,
    string Conduta,
    string? Prescricao,
    string? Encaminhamento,
    DateTimeOffset IniciadoEm,
    DateTimeOffset? FinalizadoEm);
