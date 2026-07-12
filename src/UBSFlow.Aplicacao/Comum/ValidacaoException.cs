namespace UBSFlow.Aplicacao.Comum;

public class ValidacaoException : Exception
{
    public ValidacaoException(string mensagem)
        : base(mensagem)
    {
    }
}
