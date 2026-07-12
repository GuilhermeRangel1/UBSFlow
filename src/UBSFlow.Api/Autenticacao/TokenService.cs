using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UBSFlow.Aplicacao.Autenticacao;

namespace UBSFlow.Api.Autenticacao;

public class TokenService
{
    private readonly JwtOptions options;

    public TokenService(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
    }

    public LoginResponse GerarToken(UsuarioAutenticadoResponse usuario)
    {
        var expiraEm = DateTimeOffset.UtcNow.AddMinutes(options.ExpiracaoMinutos);
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, usuario.Usuario),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Papel)
        };

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: expiraEm.UtcDateTime,
            signingCredentials: credenciais);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(tokenString, "Bearer", expiraEm, usuario);
    }
}
