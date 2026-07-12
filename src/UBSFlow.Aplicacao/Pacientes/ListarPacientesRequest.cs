namespace UBSFlow.Aplicacao.Pacientes;

public sealed record ListarPacientesRequest(
    string? Nome,
    string? Cpf);
