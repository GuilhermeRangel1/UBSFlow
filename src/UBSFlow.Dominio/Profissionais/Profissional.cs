using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Profissionais;

public class Profissional : Entidade
{
    private Profissional()
    {
        Nome = string.Empty;
        Disponibilidades = [];
    }

    public Profissional(
        string nome,
        PapelProfissional papel,
        string? especialidade,
        string? registroProfissional,
        IEnumerable<DisponibilidadeSemanal>? disponibilidades = null)
    {
        Nome = nome;
        Papel = papel;
        Especialidade = especialidade;
        RegistroProfissional = registroProfissional;
        Disponibilidades = disponibilidades?.ToList() ?? [];
    }

    public string Nome { get; private set; }
    public PapelProfissional Papel { get; private set; }
    public string? Especialidade { get; private set; }
    public string? RegistroProfissional { get; private set; }
    public IReadOnlyCollection<DisponibilidadeSemanal> Disponibilidades { get; private set; }
}
