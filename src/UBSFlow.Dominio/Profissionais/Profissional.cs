using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Profissionais;

public class Profissional : Entidade
{
    public Profissional(
        string nome,
        PapelProfissional papel,
        string? especialidade,
        string? registroProfissional)
    {
        Nome = nome;
        Papel = papel;
        Especialidade = especialidade;
        RegistroProfissional = registroProfissional;
    }

    public string Nome { get; private set; }
    public PapelProfissional Papel { get; private set; }
    public string? Especialidade { get; private set; }
    public string? RegistroProfissional { get; private set; }
}
