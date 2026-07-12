using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Dominio.Auditoria;
using Xunit;

namespace UBSFlow.Testes.Auditoria;

public class AuditoriaServiceTests
{
    [Fact]
    public void Registrar_DeveAdicionarLogDeAuditoria()
    {
        var repositorio = new AuditoriaRepositorioFake();
        var service = new AuditoriaService(repositorio);
        var entidadeId = Guid.NewGuid();

        service.Registrar(
            AcaoAuditoria.PacienteCriado,
            "Paciente",
            entidadeId,
            "Paciente cadastrado.");

        var log = Assert.Single(service.Listar());
        Assert.Equal(AcaoAuditoria.PacienteCriado, log.Acao);
        Assert.Equal("Paciente", log.Entidade);
        Assert.Equal(entidadeId, log.EntidadeId);
        Assert.Equal("sistema", log.Usuario);
        Assert.Equal("Paciente cadastrado.", log.Descricao);
    }

    [Fact]
    public void Listar_DeveRetornarLogsMaisRecentesPrimeiro()
    {
        var repositorio = new AuditoriaRepositorioFake();
        var service = new AuditoriaService(repositorio);
        var logAntigo = new LogAuditoria(
            AcaoAuditoria.PacienteCriado,
            "Paciente",
            Guid.NewGuid(),
            "sistema",
            "Paciente cadastrado.",
            new DateTimeOffset(2026, 7, 12, 8, 0, 0, TimeSpan.Zero));
        var logRecente = new LogAuditoria(
            AcaoAuditoria.AtendimentoFinalizado,
            "Atendimento",
            Guid.NewGuid(),
            "sistema",
            "Atendimento finalizado.",
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.Zero));

        repositorio.Adicionar(logAntigo);
        repositorio.Adicionar(logRecente);

        var logs = service.Listar().ToList();

        Assert.Equal(AcaoAuditoria.AtendimentoFinalizado, logs[0].Acao);
        Assert.Equal(AcaoAuditoria.PacienteCriado, logs[1].Acao);
    }

    private sealed class AuditoriaRepositorioFake : IAuditoriaRepositorio
    {
        private readonly List<LogAuditoria> logs = [];

        public IReadOnlyCollection<LogAuditoria> Listar() => logs;

        public void Adicionar(LogAuditoria log) => logs.Add(log);
    }
}
