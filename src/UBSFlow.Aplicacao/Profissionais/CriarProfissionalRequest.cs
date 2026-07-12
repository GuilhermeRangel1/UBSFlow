using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Aplicacao.Profissionais;

public sealed record CriarProfissionalRequest(
    string Nome,
    PapelProfissional Papel,
    string? Especialidade,
    string? RegistroProfissional,
    IReadOnlyCollection<DisponibilidadeSemanalRequest>? Disponibilidades = null);
