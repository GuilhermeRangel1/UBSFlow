using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Fila;
using Xunit;

namespace UBSFlow.Testes.Atendimentos;

public class AtendimentoServiceTests
{
    [Fact]
    public void Criar_DeveIniciarAtendimento()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckInAguardandoAtendimento();
        contexto.CheckIns.Adicionar(checkIn);

        var atendimento = contexto.Service.Criar(CriarRequest(checkIn.Id));

        Assert.Equal(checkIn.Id, atendimento.CheckInId);
        Assert.Equal(StatusFilaAtendimento.EmAtendimento, checkIn.Status);
        Assert.Single(contexto.Atendimentos.Listar());
    }

    [Fact]
    public void Criar_NaoDevePermitirCheckInInexistente()
    {
        var contexto = CriarContexto();

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.Criar(CriarRequest(Guid.NewGuid())));

        Assert.Equal("Check-in informado nao existe.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirAtendimentoSemTriagem()
    {
        var contexto = CriarContexto();
        var checkIn = new CheckInAtendimento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)));
        contexto.CheckIns.Adicionar(checkIn);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Criar(CriarRequest(checkIn.Id)));

        Assert.Equal("Check-in nao esta aguardando atendimento medico.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirAtendimentoDuplicado()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckInAguardandoAtendimento();
        contexto.CheckIns.Adicionar(checkIn);
        contexto.Service.Criar(CriarRequest(checkIn.Id));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Criar(CriarRequest(checkIn.Id)));

        Assert.Equal("Check-in nao esta aguardando atendimento medico.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirQueixaVazia()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckInAguardandoAtendimento();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with { Queixa = "" };

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Queixa e obrigatoria.", exception.Message);
    }

    private static CriarAtendimentoRequest CriarRequest(Guid checkInId)
    {
        return new CriarAtendimentoRequest(
            checkInId,
            "Dor no corpo e febre",
            "Sindrome viral",
            "Orientacao, hidratacao e observacao",
            "Dipirona se febre",
            null,
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)));
    }

    private static CheckInAtendimento CriarCheckInAguardandoAtendimento()
    {
        var checkIn = new CheckInAtendimento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)));
        checkIn.IniciarTriagem();
        checkIn.ConcluirTriagem();

        return checkIn;
    }

    private static ContextoTeste CriarContexto()
    {
        var atendimentos = new AtendimentoRepositorioFake();
        var checkIns = new CheckInRepositorioFake();
        var service = new AtendimentoService(atendimentos, checkIns);

        return new ContextoTeste(service, atendimentos, checkIns);
    }

    private sealed record ContextoTeste(
        AtendimentoService Service,
        AtendimentoRepositorioFake Atendimentos,
        CheckInRepositorioFake CheckIns);

    private sealed class AtendimentoRepositorioFake : IAtendimentoRepositorio
    {
        private readonly List<Atendimento> atendimentos = [];

        public IReadOnlyCollection<Atendimento> Listar() => atendimentos;

        public Atendimento? ObterPorId(Guid id)
        {
            return atendimentos.FirstOrDefault(atendimento => atendimento.Id == id);
        }

        public Atendimento? ObterPorCheckInId(Guid checkInId)
        {
            return atendimentos.FirstOrDefault(atendimento => atendimento.CheckInId == checkInId);
        }

        public void Adicionar(Atendimento atendimento)
        {
            atendimentos.Add(atendimento);
        }
    }

    private sealed class CheckInRepositorioFake : ICheckInAtendimentoRepositorio
    {
        private readonly List<CheckInAtendimento> checkIns = [];

        public IReadOnlyCollection<CheckInAtendimento> Listar() => checkIns;

        public CheckInAtendimento? ObterPorId(Guid id)
        {
            return checkIns.FirstOrDefault(checkIn => checkIn.Id == id);
        }

        public CheckInAtendimento? ObterPorAgendamentoId(Guid agendamentoId)
        {
            return checkIns.FirstOrDefault(checkIn => checkIn.AgendamentoId == agendamentoId);
        }

        public void Adicionar(CheckInAtendimento checkIn)
        {
            checkIns.Add(checkIn);
        }
    }
}
