namespace UBSFlow.Api.Autenticacao;

public class JwtOptions
{
    public string Issuer { get; set; } = "UBSFlow";
    public string Audience { get; set; } = "UBSFlow.Api";
    public string Secret { get; set; } = "UBSFlow-chave-local-de-desenvolvimento-com-mais-de-32-caracteres";
    public int ExpiracaoMinutos { get; set; } = 120;
}
