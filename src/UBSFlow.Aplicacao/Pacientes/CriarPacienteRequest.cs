namespace UBSFlow.Aplicacao.Pacientes;

public sealed record CriarPacienteRequest(
    string Nome,
    string Cpf,
    DateOnly DataNascimento,
    string Telefone,
    string? Cns);
