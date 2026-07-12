namespace UBSFlow.Aplicacao.Autenticacao;

public interface ISenhaHasher
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hash);
}
