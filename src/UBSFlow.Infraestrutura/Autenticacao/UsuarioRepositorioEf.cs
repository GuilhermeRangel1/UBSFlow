using UBSFlow.Aplicacao.Autenticacao;
using UBSFlow.Dominio.Autenticacao;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Autenticacao;

public class UsuarioRepositorioEf : IUsuarioRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public UsuarioRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public UsuarioSistema? ObterPorLogin(string login)
    {
        return dbContext.Usuarios.FirstOrDefault(usuario => usuario.Login == login.ToLowerInvariant());
    }
}
