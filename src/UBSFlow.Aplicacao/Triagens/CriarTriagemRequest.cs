using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Triagens;

public sealed record CriarTriagemRequest(
    Guid CheckInId,
    decimal Temperatura,
    int PressaoSistolica,
    int PressaoDiastolica,
    int FrequenciaCardiaca,
    string Sintomas,
    ClassificacaoRisco ClassificacaoRisco,
    string? Observacoes,
    DateTimeOffset? RealizadaEm = null);
