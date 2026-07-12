namespace UBSFlow.Aplicacao.Autenticacao;

public record LoginResponse(
    string Token,
    string Tipo,
    DateTimeOffset ExpiraEm,
    UsuarioAutenticadoResponse Usuario);
