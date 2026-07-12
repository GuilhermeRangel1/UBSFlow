using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Aplicacao.Profissionais;

public sealed record ProfissionalResponse(
    Guid Id,
    string Nome,
    PapelProfissional Papel,
    string? Especialidade,
    string? RegistroProfissional,
    IReadOnlyCollection<DisponibilidadeSemanalResponse> Disponibilidades);
