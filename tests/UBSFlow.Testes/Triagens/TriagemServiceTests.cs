using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Triagens;
using Xunit;

namespace UBSFlow.Testes.Triagens;

public class TriagemServiceTests
{
    [Fact]
    public void Criar_DeveRegistrarTriagemEMoverFilaParaAguardandoAtendimento()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);

        var triagem = contexto.Service.Criar(CriarRequest(checkIn.Id));

        Assert.Equal(checkIn.Id, triagem.CheckInId);
        Assert.Equal(ClassificacaoRisco.Amarelo, triagem.ClassificacaoRisco);
        Assert.Equal(StatusFilaAtendimento.AguardandoAtendimento, checkIn.Status);
        Assert.Single(contexto.Triagens.Listar());
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
    public void Criar_NaoDevePermitirTriagemDuplicada()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        contexto.Service.Criar(CriarRequest(checkIn.Id));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Criar(CriarRequest(checkIn.Id)));

        Assert.Equal("Check-in nao esta aguardando triagem.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirSintomasVazios()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with { Sintomas = "" };

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Sintomas sao obrigatorios.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirTemperaturaInvalida()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with { Temperatura = 50 };

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Temperatura deve estar entre 30 e 45 graus.", exception.Message);
    }

    [Fact]
    public void Criar_DeveClassificarComoVermelhoQuandoTemperaturaForCritica()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with
        {
            Temperatura = 40,
            ClassificacaoRisco = ClassificacaoRisco.Verde
        };

        var triagem = contexto.Service.Criar(request);

        Assert.Equal(ClassificacaoRisco.Vermelho, triagem.ClassificacaoRisco);
    }

    [Fact]
    public void Criar_DeveClassificarComoVermelhoQuandoSintomaForCritico()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with
        {
            Sintomas = "Paciente com dor no peito e falta de ar",
            ClassificacaoRisco = ClassificacaoRisco.Verde
        };

        var triagem = contexto.Service.Criar(request);

        Assert.Equal(ClassificacaoRisco.Vermelho, triagem.ClassificacaoRisco);
    }

    [Fact]
    public void Criar_DevePreservarClassificacaoInformadaQuandoElaForMaisGrave()
    {
        var contexto = CriarContexto();
        var checkIn = CriarCheckIn();
        contexto.CheckIns.Adicionar(checkIn);
        var request = CriarRequest(checkIn.Id) with
        {
            Temperatura = 36.5m,
            PressaoSistolica = 120,
            PressaoDiastolica = 80,
            FrequenciaCardiaca = 80,
            ClassificacaoRisco = ClassificacaoRisco.Laranja
        };

        var triagem = contexto.Service.Criar(request);

        Assert.Equal(ClassificacaoRisco.Laranja, triagem.ClassificacaoRisco);
    }

    private static CriarTriagemRequest CriarRequest(Guid checkInId)
    {
        return new CriarTriagemRequest(
            checkInId,
            37.8m,
            130,
            85,
            92,
            "Febre e dor no corpo",
            ClassificacaoRisco.Amarelo,
            "Paciente relata sintomas ha dois dias",
            new DateTimeOffset(2026, 7, 12, 9, 20, 0, TimeSpan.FromHours(-3)));
    }

    private static CheckInAtendimento CriarCheckIn()
    {
        return new CheckInAtendimento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)));
    }

    private static ContextoTeste CriarContexto()
    {
        var triagens = new TriagemRepositorioFake();
        var checkIns = new CheckInRepositorioFake();
        var service = new TriagemService(triagens, checkIns);

        return new ContextoTeste(service, triagens, checkIns);
    }

    private sealed record ContextoTeste(
        TriagemService Service,
        TriagemRepositorioFake Triagens,
        CheckInRepositorioFake CheckIns);

    private sealed class TriagemRepositorioFake : ITriagemRepositorio
    {
        private readonly List<Triagem> triagens = [];

        public IReadOnlyCollection<Triagem> Listar() => triagens;

        public Triagem? ObterPorId(Guid id)
        {
            return triagens.FirstOrDefault(triagem => triagem.Id == id);
        }

        public Triagem? ObterPorCheckInId(Guid checkInId)
        {
            return triagens.FirstOrDefault(triagem => triagem.CheckInId == checkInId);
        }

        public void Adicionar(Triagem triagem)
        {
            triagens.Add(triagem);
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
