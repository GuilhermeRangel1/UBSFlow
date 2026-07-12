using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Aplicacao.Profissionais;

public sealed record ListarProfissionaisRequest(
    string? Nome,
    PapelProfissional? Papel,
    int Pagina = 1,
    int TamanhoPagina = 10);
