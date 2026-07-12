using UBSFlow.Aplicacao.Comum;

namespace UBSFlow.Aplicacao.Autenticacao;

public class AuthService
{
    private static readonly IReadOnlyCollection<UsuarioSistema> Usuarios =
    [
        new UsuarioSistema(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Administrador", "admin", "admin123", "ADMIN"),
        new UsuarioSistema(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Recepcao UBS", "recepcao", "recepcao123", "RECEPCIONISTA"),
        new UsuarioSistema(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Enfermagem UBS", "enfermagem", "enfermagem123", "ENFERMEIRO"),
        new UsuarioSistema(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Medico UBS", "medico", "medico123", "MEDICO"),
        new UsuarioSistema(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Gestao UBS", "gestao", "gestao123", "GESTOR")
    ];

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

        var usuario = Usuarios.FirstOrDefault(usuario =>
            usuario.Login.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase) &&
            usuario.Senha == request.Senha);

        if (usuario is null)
        {
            throw new UnauthorizedAccessException("Usuario ou senha invalidos.");
        }

        return new UsuarioAutenticadoResponse(
            usuario.Id,
            usuario.Nome,
            usuario.Login,
            usuario.Papel);
    }

    private sealed record UsuarioSistema(
        Guid Id,
        string Nome,
        string Login,
        string Senha,
        string Papel);
}
