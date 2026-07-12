namespace UBSFlow.Aplicacao.Pacientes;

public sealed record PacienteResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string? Cns,
    DateOnly DataNascimento,
    string Telefone);
