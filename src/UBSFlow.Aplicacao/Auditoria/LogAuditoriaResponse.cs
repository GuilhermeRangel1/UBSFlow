using UBSFlow.Dominio.Auditoria;

namespace UBSFlow.Aplicacao.Auditoria;

public record LogAuditoriaResponse(
    Guid Id,
    AcaoAuditoria Acao,
    string Entidade,
    Guid EntidadeId,
    string Usuario,
    string Descricao,
    DateTimeOffset RegistradoEm);
