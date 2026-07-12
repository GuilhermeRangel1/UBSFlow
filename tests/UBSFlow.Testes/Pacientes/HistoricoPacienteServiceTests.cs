using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Pacientes;
using UBSFlow.Dominio.Triagens;
using Xunit;

namespace UBSFlow.Testes.Pacientes;

public class HistoricoPacienteServiceTests
{
    [Fact]
    public void Obter_DeveRetornarHistoricoDoPaciente()
    {
        var contexto = CriarContexto();
        var paciente = new Paciente("Maria Silva", "12345678901", new DateOnly(1990, 5, 12), "11999990000");
        var profissionalId = Guid.NewGuid();
        var agendamento = new Agendamento(
            paciente.Id,
            profissionalId,
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
        var checkIn = new CheckInAtendimento(
            agendamento.Id,
            paciente.Id,
            profissionalId,
            new DateTimeOffset(2026, 7, 12, 8, 50, 0, TimeSpan.FromHours(-3)));
        var triagem = new Triagem(
            checkIn.Id,
            paciente.Id,
            37.8m,
            130,
            85,
            92,
            "Febre",
            ClassificacaoRisco.Amarelo,
            null,
            new DateTimeOffset(2026, 7, 12, 9, 10, 0, TimeSpan.FromHours(-3)));
        var atendimento = new Atendimento(
            checkIn.Id,
            paciente.Id,
            profissionalId,
            "Febre",
            "Sindrome viral",
            "Hidratacao",
            null,
            null,
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
        contexto.Pacientes.Adicionar(paciente);
        contexto.Agendamentos.Adicionar(agendamento);
        contexto.CheckIns.Adicionar(checkIn);
        contexto.Triagens.Adicionar(triagem);
        contexto.Atendimentos.Adicionar(atendimento);

        var historico = contexto.Service.Obter(paciente.Id);

        Assert.Equal(paciente.Id, historico.PacienteId);
        Assert.Single(historico.Agendamentos);
        Assert.Single(historico.Triagens);
        Assert.Single(historico.Atendimentos);
    }

    [Fact]
    public void Obter_NaoDevePermitirPacienteInexistente()
    {
        var contexto = CriarContexto();

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Obter(Guid.NewGuid()));

        Assert.Equal("Paciente informado nao existe.", exception.Message);
    }

    private static ContextoTeste CriarContexto()
    {
        var pacientes = new PacienteRepositorioFake();
        var agendamentos = new AgendamentoRepositorioFake();
        var checkIns = new CheckInRepositorioFake();
        var triagens = new TriagemRepositorioFake();
        var atendimentos = new AtendimentoRepositorioFake();
        var service = new HistoricoPacienteService(pacientes, agendamentos, checkIns, triagens, atendimentos);

        return new ContextoTeste(service, pacientes, agendamentos, checkIns, triagens, atendimentos);
    }

    private sealed record ContextoTeste(
        HistoricoPacienteService Service,
        PacienteRepositorioFake Pacientes,
        AgendamentoRepositorioFake Agendamentos,
        CheckInRepositorioFake CheckIns,
        TriagemRepositorioFake Triagens,
        AtendimentoRepositorioFake Atendimentos);

    private sealed class PacienteRepositorioFake : IPacienteRepositorio
    {
        private readonly List<Paciente> pacientes = [];
        public IReadOnlyCollection<Paciente> Listar() => pacientes;
        public Paciente? ObterPorId(Guid id) => pacientes.FirstOrDefault(paciente => paciente.Id == id);
        public Paciente? ObterPorCpf(string cpf) => pacientes.FirstOrDefault(paciente => paciente.Cpf == cpf);
        public void Adicionar(Paciente paciente) => pacientes.Add(paciente);
    }

    private sealed class AgendamentoRepositorioFake : IAgendamentoRepositorio
    {
        private readonly List<Agendamento> agendamentos = [];
        public IReadOnlyCollection<Agendamento> Listar() => agendamentos;
        public Agendamento? ObterPorId(Guid id) => agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
        public void Adicionar(Agendamento agendamento) => agendamentos.Add(agendamento);
        public void SalvarAlteracoes() { }
    }

    private sealed class CheckInRepositorioFake : ICheckInAtendimentoRepositorio
    {
        private readonly List<CheckInAtendimento> checkIns = [];
        public IReadOnlyCollection<CheckInAtendimento> Listar() => checkIns;
        public CheckInAtendimento? ObterPorId(Guid id) => checkIns.FirstOrDefault(checkIn => checkIn.Id == id);
        public CheckInAtendimento? ObterPorAgendamentoId(Guid agendamentoId) =>
            checkIns.FirstOrDefault(checkIn => checkIn.AgendamentoId == agendamentoId);
        public void Adicionar(CheckInAtendimento checkIn) => checkIns.Add(checkIn);
        public void SalvarAlteracoes() { }
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

    private sealed class AtendimentoRepositorioFake : IAtendimentoRepositorio
    {
        private readonly List<Atendimento> atendimentos = [];
        public IReadOnlyCollection<Atendimento> Listar() => atendimentos;
        public Atendimento? ObterPorId(Guid id) =>
            atendimentos.FirstOrDefault(atendimento => atendimento.Id == id);
        public Atendimento? ObterPorCheckInId(Guid checkInId) =>
            atendimentos.FirstOrDefault(atendimento => atendimento.CheckInId == checkInId);
        public void Adicionar(Atendimento atendimento) => atendimentos.Add(atendimento);
        public void SalvarAlteracoes() { }
    }
}
