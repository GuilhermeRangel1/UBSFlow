using UBSFlow.Aplicacao.Autenticacao;
using UBSFlow.Aplicacao.Comum;
using Xunit;

namespace UBSFlow.Testes.Autenticacao;

public class AuthServiceTests
{
    [Fact]
    public void Autenticar_DeveRetornarUsuarioQuandoCredenciaisForemValidas()
    {
        var service = new AuthService();

        var usuario = service.Autenticar(new LoginRequest("medico", "medico123"));

        Assert.Equal("Medico UBS", usuario.Nome);
        Assert.Equal("medico", usuario.Usuario);
        Assert.Equal("MEDICO", usuario.Papel);
    }

    [Fact]
    public void Autenticar_DeveRejeitarSenhaInvalida()
    {
        var service = new AuthService();

        var exception = Assert.Throws<UnauthorizedAccessException>(() =>
            service.Autenticar(new LoginRequest("medico", "senha-errada")));

        Assert.Equal("Usuario ou senha invalidos.", exception.Message);
    }

    [Fact]
    public void Autenticar_DeveValidarUsuarioObrigatorio()
    {
        var service = new AuthService();

        var exception = Assert.Throws<ValidacaoException>(() =>
            service.Autenticar(new LoginRequest("", "medico123")));

        Assert.Equal("Usuario e obrigatorio.", exception.Message);
    }
}
