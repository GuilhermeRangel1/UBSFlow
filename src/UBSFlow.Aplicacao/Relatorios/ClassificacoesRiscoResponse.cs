using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Relatorios;

public sealed record ClassificacoesRiscoResponse(
    DateOnly Inicio,
    DateOnly Fim,
    IReadOnlyCollection<ClassificacaoRiscoItemResponse> Itens);

public sealed record ClassificacaoRiscoItemResponse(
    ClassificacaoRisco ClassificacaoRisco,
    int Total);
