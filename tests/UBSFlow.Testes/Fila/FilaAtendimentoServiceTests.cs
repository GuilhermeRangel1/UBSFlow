using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Triagens;
using Xunit;

namespace UBSFlow.Testes.Fila;

public class FilaAtendimentoServiceTests
{
    [Fact]
    public void CriarCheckIn_DeveAdicionarPacienteNaFila()
    {
        var contexto = CriarContexto();
        var agendamento = CriarAgendamento();
        contexto.Agendamentos.Adicionar(agendamento);

        var checkIn = contexto.Service.CriarCheckIn(new CriarCheckInRequest(agendamento.Id));

        Assert.Equal(StatusFilaAtendimento.AguardandoTriagem, checkIn.Status);
        Assert.Equal(agendamento.PacienteId, checkIn.PacienteId);
        Assert.Single(contexto.CheckIns.Listar());
    }

    [Fact]
    public void CriarCheckIn_NaoDevePermitirAgendamentoInexistente()
    {
        var contexto = CriarContexto();

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.CriarCheckIn(new CriarCheckInRequest(Guid.NewGuid())));

        Assert.Equal("Agendamento informado nao existe.", exception.Message);
    }

    [Fact]
    public void CriarCheckIn_NaoDevePermitirAgendamentoCancelado()
    {
        var contexto = CriarContexto();
        var agendamento = CriarAgendamento();
        agendamento.Cancelar("Paciente solicitou cancelamento.");
        contexto.Agendamentos.Adicionar(agendamento);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.CriarCheckIn(new CriarCheckInRequest(agendamento.Id)));

        Assert.Equal("Nao e possivel fazer check-in de agendamento cancelado.", exception.Message);
    }

    [Fact]
    public void CriarCheckIn_NaoDevePermitirCheckInDuplicado()
    {
        var contexto = CriarContexto();
        var agendamento = CriarAgendamento();
        contexto.Agendamentos.Adicionar(agendamento);
        contexto.Service.CriarCheckIn(new CriarCheckInRequest(agendamento.Id));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.CriarCheckIn(new CriarCheckInRequest(agendamento.Id)));

        Assert.Equal("Check-in ja realizado para este agendamento.", exception.Message);
    }

    [Fact]
    public void ListarFila_DeveOrdenarPorHorarioDeChegada()
    {
        var contexto = CriarContexto();
        var primeiroAgendamento = CriarAgendamento();
        var segundoAgendamento = CriarAgendamento();
        contexto.Agendamentos.Adicionar(primeiroAgendamento);
        contexto.Agendamentos.Adicionar(segundoAgendamento);
        var data = new DateOnly(2026, 7, 12);
        contexto.Service.CriarCheckIn(new CriarCheckInRequest(
            segundoAgendamento.Id,
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3))));
        contexto.Service.CriarCheckIn(new CriarCheckInRequest(
            primeiroAgendamento.Id,
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3))));

        var fila = contexto.Service.ListarFila(new ListarFilaRequest(data));

        Assert.Collection(
            fila,
            primeiro => Assert.Equal(primeiroAgendamento.Id, primeiro.AgendamentoId),
            segundo => Assert.Equal(segundoAgendamento.Id, segundo.AgendamentoId));
    }

    [Fact]
    public void ListarFila_DevePriorizarPacientesTriadosPorClassificacaoDeRisco()
    {
        var contexto = CriarContexto();
        var primeiroAgendamento = CriarAgendamento();
        var segundoAgendamento = CriarAgendamento();
        contexto.Agendamentos.Adicionar(primeiroAgendamento);
        contexto.Agendamentos.Adicionar(segundoAgendamento);
        var primeiroCheckIn = contexto.Service.CriarCheckIn(new CriarCheckInRequest(
            primeiroAgendamento.Id,
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3))));
        var segundoCheckIn = contexto.Service.CriarCheckIn(new CriarCheckInRequest(
            segundoAgendamento.Id,
            new DateTimeOffset(2026, 7, 12, 9, 10, 0, TimeSpan.FromHours(-3))));
        var primeiroCheckInDominio = contexto.CheckIns.ObterPorId(primeiroCheckIn.Id)!;
        var segundoCheckInDominio = contexto.CheckIns.ObterPorId(segundoCheckIn.Id)!;
        primeiroCheckInDominio.IniciarTriagem();
        primeiroCheckInDominio.ConcluirTriagem();
        segundoCheckInDominio.IniciarTriagem();
        segundoCheckInDominio.ConcluirTriagem();
        contexto.Triagens.Adicionar(CriarTriagem(primeiroCheckIn.Id, ClassificacaoRisco.Amarelo));
        contexto.Triagens.Adicionar(CriarTriagem(segundoCheckIn.Id, ClassificacaoRisco.Vermelho));

        var fila = contexto.Service.ListarFila(new ListarFilaRequest(new DateOnly(2026, 7, 12)));

        Assert.Collection(
            fila,
            primeiro =>
            {
                Assert.Equal(segundoCheckIn.Id, primeiro.Id);
                Assert.Equal(ClassificacaoRisco.Vermelho, primeiro.ClassificacaoRisco);
            },
            segundo =>
            {
                Assert.Equal(primeiroCheckIn.Id, segundo.Id);
                Assert.Equal(ClassificacaoRisco.Amarelo, segundo.ClassificacaoRisco);
            });
    }

    private static Agendamento CriarAgendamento()
    {
        return new Agendamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
    }

    private static Triagem CriarTriagem(Guid checkInId, ClassificacaoRisco classificacaoRisco)
    {
        return new Triagem(
            checkInId,
            Guid.NewGuid(),
            37.8m,
            130,
            85,
            92,
            "Febre",
            classificacaoRisco,
            null,
            new DateTimeOffset(2026, 7, 12, 9, 20, 0, TimeSpan.FromHours(-3)));
    }

    private static ContextoTeste CriarContexto()
    {
        var checkIns = new CheckInRepositorioFake();
        var agendamentos = new AgendamentoRepositorioFake();
        var triagens = new TriagemRepositorioFake();
        var service = new FilaAtendimentoService(checkIns, agendamentos, triagens);

        return new ContextoTeste(service, checkIns, agendamentos, triagens);
    }

    private sealed record ContextoTeste(
        FilaAtendimentoService Service,
        CheckInRepositorioFake CheckIns,
        AgendamentoRepositorioFake Agendamentos,
        TriagemRepositorioFake Triagens);

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

    private sealed class AgendamentoRepositorioFake : IAgendamentoRepositorio
    {
        private readonly List<Agendamento> agendamentos = [];

        public IReadOnlyCollection<Agendamento> Listar() => agendamentos;

        public Agendamento? ObterPorId(Guid id)
        {
            return agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
        }

        public void Adicionar(Agendamento agendamento)
        {
            agendamentos.Add(agendamento);
        }

        public void SalvarAlteracoes()
        {
        }
    }

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
}
