using UBSFlow.Dominio.Auditoria;

namespace UBSFlow.Aplicacao.Auditoria;

public class AuditoriaService
{
    private readonly IAuditoriaRepositorio auditoriaRepositorio;

    public AuditoriaService(IAuditoriaRepositorio auditoriaRepositorio)
    {
        this.auditoriaRepositorio = auditoriaRepositorio;
    }

    public IReadOnlyCollection<LogAuditoriaResponse> Listar()
    {
        return auditoriaRepositorio
            .Listar()
            .OrderByDescending(log => log.RegistradoEm)
            .Select(MapearLog)
            .ToList();
    }

    public void Registrar(
        AcaoAuditoria acao,
        string entidade,
        Guid entidadeId,
        string descricao,
        string usuario = "sistema")
    {
        var log = new LogAuditoria(
            acao,
            entidade,
            entidadeId,
            usuario,
            descricao,
            DateTimeOffset.UtcNow);

        auditoriaRepositorio.Adicionar(log);
    }

    private static LogAuditoriaResponse MapearLog(LogAuditoria log)
    {
        return new LogAuditoriaResponse(
            log.Id,
            log.Acao,
            log.Entidade,
            log.EntidadeId,
            log.Usuario,
            log.Descricao,
            log.RegistradoEm);
    }
}
