using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Auditoria;

public class LogAuditoria : Entidade
{
    private LogAuditoria()
    {
        Entidade = string.Empty;
        Usuario = string.Empty;
        Descricao = string.Empty;
    }

    public LogAuditoria(
        AcaoAuditoria acao,
        string entidade,
        Guid entidadeId,
        string usuario,
        string descricao,
        DateTimeOffset registradoEm)
    {
        Acao = acao;
        Entidade = entidade;
        EntidadeId = entidadeId;
        Usuario = usuario;
        Descricao = descricao;
        RegistradoEm = registradoEm;
    }

    public AcaoAuditoria Acao { get; private set; }
    public string Entidade { get; private set; }
    public Guid EntidadeId { get; private set; }
    public string Usuario { get; private set; }
    public string Descricao { get; private set; }
    public DateTimeOffset RegistradoEm { get; private set; }
}
