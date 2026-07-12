using UBSFlow.Aplicacao.Autenticacao;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Dominio.Autenticacao;
using Xunit;

namespace UBSFlow.Testes.Autenticacao;

public class AuthServiceTests
{
    [Fact]
    public void Autenticar_DeveRetornarUsuarioQuandoCredenciaisForemValidas()
    {
        var service = CriarService();

        var usuario = service.Autenticar(new LoginRequest("medico", "medico123"));

        Assert.Equal("Medico UBS", usuario.Nome);
        Assert.Equal("medico", usuario.Usuario);
        Assert.Equal("MEDICO", usuario.Papel);
    }

    [Fact]
    public void Autenticar_DeveRejeitarSenhaInvalida()
    {
        var service = CriarService();

        var exception = Assert.Throws<UnauthorizedAccessException>(() =>
            service.Autenticar(new LoginRequest("medico", "senha-errada")));

        Assert.Equal("Usuario ou senha invalidos.", exception.Message);
    }

    [Fact]
    public void Autenticar_DeveValidarUsuarioObrigatorio()
    {
        var service = CriarService();

        var exception = Assert.Throws<ValidacaoException>(() =>
            service.Autenticar(new LoginRequest("", "medico123")));

        Assert.Equal("Usuario e obrigatorio.", exception.Message);
    }

    private static AuthService CriarService()
    {
        var senhaHasher = new SenhaHasher();
        var repositorio = new UsuarioRepositorioFake(
            new UsuarioSistema(
                "Medico UBS",
                "medico",
                senhaHasher.GerarHash("medico123"),
                "MEDICO"));

        return new AuthService(repositorio, senhaHasher);
    }

    private sealed class UsuarioRepositorioFake : IUsuarioRepositorio
    {
        private readonly UsuarioSistema usuario;

        public UsuarioRepositorioFake(UsuarioSistema usuario)
        {
            this.usuario = usuario;
        }

        public UsuarioSistema? ObterPorLogin(string login)
        {
            return usuario.Login.Equals(login, StringComparison.OrdinalIgnoreCase) ? usuario : null;
        }
    }
}
