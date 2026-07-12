using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Dominio.Auditoria;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Auditoria;

public class AuditoriaRepositorioEf : IAuditoriaRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public AuditoriaRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<LogAuditoria> Listar()
    {
        return dbContext
            .LogsAuditoria
            .AsNoTracking()
            .OrderByDescending(log => log.RegistradoEm)
            .ToList();
    }

    public void Adicionar(LogAuditoria log)
    {
        dbContext.LogsAuditoria.Add(log);
        dbContext.SaveChanges();
    }
}
