using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Dominio.Auditoria;

namespace UBSFlow.Infraestrutura.Auditoria;

public class AuditoriaRepositorioEmMemoria : IAuditoriaRepositorio
{
    private static readonly List<LogAuditoria> Logs = [];

    public IReadOnlyCollection<LogAuditoria> Listar()
    {
        return Logs;
    }

    public void Adicionar(LogAuditoria log)
    {
        Logs.Add(log);
    }
}
