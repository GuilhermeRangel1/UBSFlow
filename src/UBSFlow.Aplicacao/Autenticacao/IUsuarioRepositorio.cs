using UBSFlow.Dominio.Autenticacao;

namespace UBSFlow.Aplicacao.Autenticacao;

public interface IUsuarioRepositorio
{
    UsuarioSistema? ObterPorLogin(string login);
}
