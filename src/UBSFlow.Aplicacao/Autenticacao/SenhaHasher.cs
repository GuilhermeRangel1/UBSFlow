using System.Security.Cryptography;
using System.Text;

namespace UBSFlow.Aplicacao.Autenticacao;

public class SenhaHasher : ISenhaHasher
{
    public string GerarHash(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hashBytes = sha256.ComputeHash(bytes);

        return string.Concat(hashBytes.Select(item => item.ToString("x2")));
    }

    public bool Verificar(string senha, string hash)
    {
        return GerarHash(senha).Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
