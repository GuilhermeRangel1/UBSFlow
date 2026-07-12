using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Triagens;

public sealed record TriagemResponse(
    Guid Id,
    Guid CheckInId,
    Guid PacienteId,
    decimal Temperatura,
    int PressaoSistolica,
    int PressaoDiastolica,
    int FrequenciaCardiaca,
    string Sintomas,
    ClassificacaoRisco ClassificacaoRisco,
    string? Observacoes,
    DateTimeOffset RealizadaEm);
