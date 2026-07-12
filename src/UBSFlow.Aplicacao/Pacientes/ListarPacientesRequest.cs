namespace UBSFlow.Aplicacao.Pacientes;

public sealed record ListarPacientesRequest(
    string? Nome,
    string? Cpf,
    int Pagina = 1,
    int TamanhoPagina = 10);
