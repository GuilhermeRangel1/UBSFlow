using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Autenticacao;

public class UsuarioSistema : Entidade
{
    private UsuarioSistema()
    {
        Nome = string.Empty;
        Login = string.Empty;
        SenhaHash = string.Empty;
        Papel = string.Empty;
    }

    public UsuarioSistema(
        string nome,
        string login,
        string senhaHash,
        string papel,
        bool ativo = true)
    {
        Nome = nome;
        Login = login;
        SenhaHash = senhaHash;
        Papel = papel;
        Ativo = ativo;
    }

    public string Nome { get; private set; }
    public string Login { get; private set; }
    public string SenhaHash { get; private set; }
    public string Papel { get; private set; }
    public bool Ativo { get; private set; }
}
