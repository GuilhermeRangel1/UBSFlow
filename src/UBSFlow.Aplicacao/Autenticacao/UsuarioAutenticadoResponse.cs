namespace UBSFlow.Aplicacao.Autenticacao;

public record UsuarioAutenticadoResponse(
    Guid Id,
    string Nome,
    string Usuario,
    string Papel);
