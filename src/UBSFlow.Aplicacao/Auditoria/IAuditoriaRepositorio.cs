using UBSFlow.Dominio.Auditoria;

namespace UBSFlow.Aplicacao.Auditoria;

public interface IAuditoriaRepositorio
{
    IReadOnlyCollection<LogAuditoria> Listar();
    void Adicionar(LogAuditoria log);
}
