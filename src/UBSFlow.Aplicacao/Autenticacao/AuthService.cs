using UBSFlow.Aplicacao.Comum;

namespace UBSFlow.Aplicacao.Autenticacao;

public class AuthService
{
    private readonly ISenhaHasher senhaHasher;
    private readonly IUsuarioRepositorio usuarioRepositorio;

    public AuthService(
        IUsuarioRepositorio usuarioRepositorio,
        ISenhaHasher senhaHasher)
    {
        this.usuarioRepositorio = usuarioRepositorio;
        this.senhaHasher = senhaHasher;
    }

    public UsuarioAutenticadoResponse Autenticar(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
        {
            throw new ValidacaoException("Usuario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Senha))
        {
            throw new ValidacaoException("Senha e obrigatoria.");
        }

        var usuario = usuarioRepositorio.ObterPorLogin(request.Usuario);

        if (usuario is null ||
            !usuario.Ativo ||
            !senhaHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            throw new UnauthorizedAccessException("Usuario ou senha invalidos.");
        }

        return new UsuarioAutenticadoResponse(
            usuario.Id,
            usuario.Nome,
            usuario.Login,
            usuario.Papel);
    }
}
