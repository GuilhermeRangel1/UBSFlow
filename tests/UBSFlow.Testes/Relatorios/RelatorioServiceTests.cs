using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Relatorios;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Triagens;
using Xunit;

namespace UBSFlow.Testes.Relatorios;

public class RelatorioServiceTests
{
    [Fact]
    public void ObterAtendimentosPorPeriodo_DeveContarAtendimentos()
    {
        var contexto = CriarContexto();
        var atendimentoFinalizado = CriarAtendimento(new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)));
        atendimentoFinalizado.Finalizar(new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
        contexto.Atendimentos.Adicionar(atendimentoFinalizado);
        contexto.Atendimentos.Adicionar(CriarAtendimento(new DateTimeOffset(2026, 7, 13, 9, 0, 0, TimeSpan.FromHours(-3))));

        var relatorio = contexto.Service.ObterAtendimentosPorPeriodo(
            new RelatorioPeriodoRequest(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31)));

        Assert.Equal(2, relatorio.TotalAtendimentos);
        Assert.Equal(1, relatorio.TotalFinalizados);
    }

    [Fact]
    public void ObterClassificacoesRisco_DeveAgruparTriagensPorClassificacao()
    {
        var contexto = CriarContexto();
        contexto.Triagens.Adicionar(CriarTriagem(ClassificacaoRisco.Amarelo));
        contexto.Triagens.Adicionar(CriarTriagem(ClassificacaoRisco.Amarelo));
        contexto.Triagens.Adicionar(CriarTriagem(ClassificacaoRisco.Vermelho));

        var relatorio = contexto.Service.ObterClassificacoesRisco(
            new RelatorioPeriodoRequest(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31)));

        Assert.Contains(relatorio.Itens, item =>
            item.ClassificacaoRisco == ClassificacaoRisco.Amarelo && item.Total == 2);
        Assert.Contains(relatorio.Itens, item =>
            item.ClassificacaoRisco == ClassificacaoRisco.Vermelho && item.Total == 1);
    }

    [Fact]
    public void ObterCancelamentos_DeveContarAgendamentosCancelados()
    {
        var contexto = CriarContexto();
        var cancelado = CriarAgendamento();
        cancelado.Cancelar("Paciente solicitou cancelamento.");
        contexto.Agendamentos.Adicionar(cancelado);
        contexto.Agendamentos.Adicionar(CriarAgendamento());

        var relatorio = contexto.Service.ObterCancelamentos(
            new RelatorioPeriodoRequest(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31)));

        Assert.Equal(1, relatorio.TotalCancelamentos);
    }

    [Fact]
    public void Relatorios_NaoDevemPermitirPeriodoInvalido()
    {
        var contexto = CriarContexto();

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.ObterAtendimentosPorPeriodo(
                new RelatorioPeriodoRequest(new DateOnly(2026, 7, 31), new DateOnly(2026, 7, 1))));

        Assert.Equal("Data final nao pode ser anterior a data inicial.", exception.Message);
    }

    private static ContextoTeste CriarContexto()
    {
        var atendimentos = new AtendimentoRepositorioFake();
        var triagens = new TriagemRepositorioFake();
        var agendamentos = new AgendamentoRepositorioFake();
        var service = new RelatorioService(atendimentos, triagens, agendamentos);

        return new ContextoTeste(service, atendimentos, triagens, agendamentos);
    }

    private static Atendimento CriarAtendimento(DateTimeOffset iniciadoEm)
    {
        return new Atendimento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Febre",
            "Sindrome viral",
            "Hidratacao",
            null,
            null,
            iniciadoEm);
    }

    private static Triagem CriarTriagem(ClassificacaoRisco classificacaoRisco)
    {
        return new Triagem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            37.8m,
            130,
            85,
            92,
            "Febre",
            classificacaoRisco,
            null,
            new DateTimeOffset(2026, 7, 12, 9, 10, 0, TimeSpan.FromHours(-3)));
    }

    private static Agendamento CriarAgendamento()
    {
        return new Agendamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
    }

    private sealed record ContextoTeste(
        RelatorioService Service,
        AtendimentoRepositorioFake Atendimentos,
        TriagemRepositorioFake Triagens,
        AgendamentoRepositorioFake Agendamentos);

    private sealed class AtendimentoRepositorioFake : IAtendimentoRepositorio
    {
        private readonly List<Atendimento> atendimentos = [];
        public IReadOnlyCollection<Atendimento> Listar() => atendimentos;
        public Atendimento? ObterPorId(Guid id) => atendimentos.FirstOrDefault(atendimento => atendimento.Id == id);
        public Atendimento? ObterPorCheckInId(Guid checkInId) =>
            atendimentos.FirstOrDefault(atendimento => atendimento.CheckInId == checkInId);
        public void Adicionar(Atendimento atendimento) => atendimentos.Add(atendimento);
    }

    private sealed class TriagemRepositorioFake : ITriagemRepositorio
    {
        private readonly List<Triagem> triagens = [];
        public IReadOnlyCollection<Triagem> Listar() => triagens;
        public Triagem? ObterPorId(Guid id) => triagens.FirstOrDefault(triagem => triagem.Id == id);
        public Triagem? ObterPorCheckInId(Guid checkInId) =>
            triagens.FirstOrDefault(triagem => triagem.CheckInId == checkInId);
        public void Adicionar(Triagem triagem) => triagens.Add(triagem);
    }

    private sealed class AgendamentoRepositorioFake : IAgendamentoRepositorio
    {
        private readonly List<Agendamento> agendamentos = [];
        public IReadOnlyCollection<Agendamento> Listar() => agendamentos;
        public Agendamento? ObterPorId(Guid id) => agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
        public void Adicionar(Agendamento agendamento) => agendamentos.Add(agendamento);
    }
}
