using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Pacientes;
using UBSFlow.Dominio.Profissionais;
using Xunit;

namespace UBSFlow.Testes.Agenda;

public class AgendamentoServiceTests
{
    [Fact]
    public void Criar_DeveCadastrarAgendamento()
    {
        var contexto = CriarContexto();
        var request = CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id);

        var agendamento = contexto.Service.Criar(request);

        Assert.Equal(StatusAgendamento.Agendado, agendamento.Status);
        Assert.Single(contexto.Agendamentos.Listar());
    }

    [Fact]
    public void Criar_NaoDevePermitirPacienteInexistente()
    {
        var contexto = CriarContexto();
        var request = CriarRequest(Guid.NewGuid(), contexto.Profissional.Id);

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Paciente informado nao existe.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirProfissionalInexistente()
    {
        var contexto = CriarContexto();
        var request = CriarRequest(contexto.Paciente.Id, Guid.NewGuid());

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Profissional informado nao existe.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirHorarioFinalAntesDoInicial()
    {
        var contexto = CriarContexto();
        var inicio = new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3));
        var fim = new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3));
        var request = new CriarAgendamentoRequest(contexto.Paciente.Id, contexto.Profissional.Id, inicio, fim);

        var exception = Assert.Throws<ValidacaoException>(() => contexto.Service.Criar(request));
        Assert.Equal("Horario final deve ser maior que o horario inicial.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirConflitoDeHorarioParaMesmoProfissional()
    {
        var contexto = CriarContexto();
        contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var requestComConflito = new CriarAgendamentoRequest(
            contexto.Paciente.Id,
            contexto.Profissional.Id,
            new DateTimeOffset(2026, 7, 12, 9, 15, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 45, 0, TimeSpan.FromHours(-3)));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Criar(requestComConflito));

        Assert.Equal("Profissional ja possui agendamento neste horario.", exception.Message);
    }

    [Fact]
    public void Listar_DeveFiltrarPorProfissional()
    {
        var contexto = CriarContexto();
        var outroProfissional = new Profissional(
            "Dra Bia",
            PapelProfissional.Medico,
            "Clinica Geral",
            "CRM54321");
        contexto.Profissionais.Adicionar(outroProfissional);
        contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        contexto.Service.Criar(CriarRequest(
            contexto.Paciente.Id,
            outroProfissional.Id,
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 10, 30, 0, TimeSpan.FromHours(-3))));

        var resultado = contexto.Service.Listar(
            new ListarAgendamentosRequest(null, outroProfissional.Id, null));

        var agendamento = Assert.Single(resultado.Itens);
        Assert.Equal(outroProfissional.Id, agendamento.ProfissionalId);
    }

    [Fact]
    public void Remarcar_DeveAlterarHorarioEStatus()
    {
        var contexto = CriarContexto();
        var agendamento = contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var request = new RemarcarAgendamentoRequest(
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 10, 30, 0, TimeSpan.FromHours(-3)));

        var agendamentoRemarcado = contexto.Service.Remarcar(agendamento.Id, request);

        Assert.Equal(StatusAgendamento.Remarcado, agendamentoRemarcado.Status);
        Assert.Equal(request.Inicio, agendamentoRemarcado.Inicio);
        Assert.Equal(request.Fim, agendamentoRemarcado.Fim);
    }

    [Fact]
    public void Remarcar_NaoDevePermitirAgendamentoInexistente()
    {
        var contexto = CriarContexto();
        var request = new RemarcarAgendamentoRequest(
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 10, 30, 0, TimeSpan.FromHours(-3)));

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.Remarcar(Guid.NewGuid(), request));

        Assert.Equal("Agendamento informado nao existe.", exception.Message);
    }

    [Fact]
    public void Remarcar_NaoDevePermitirHorarioFinalAntesDoInicial()
    {
        var contexto = CriarContexto();
        var agendamento = contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var request = new RemarcarAgendamentoRequest(
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)));

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.Remarcar(agendamento.Id, request));

        Assert.Equal("Horario final deve ser maior que o horario inicial.", exception.Message);
    }

    [Fact]
    public void Remarcar_NaoDevePermitirConflitoComOutroAgendamento()
    {
        var contexto = CriarContexto();
        var primeiroAgendamento = contexto.Service.Criar(
            CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        contexto.Service.Criar(CriarRequest(
            contexto.Paciente.Id,
            contexto.Profissional.Id,
            new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 10, 30, 0, TimeSpan.FromHours(-3))));
        var request = new RemarcarAgendamentoRequest(
            new DateTimeOffset(2026, 7, 12, 10, 15, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 10, 45, 0, TimeSpan.FromHours(-3)));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Remarcar(primeiroAgendamento.Id, request));

        Assert.Equal("Profissional ja possui agendamento neste horario.", exception.Message);
    }

    [Fact]
    public void Cancelar_DeveAlterarStatusERegistrarMotivo()
    {
        var contexto = CriarContexto();
        var agendamento = contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var request = new CancelarAgendamentoRequest("Paciente solicitou cancelamento.");

        var agendamentoCancelado = contexto.Service.Cancelar(agendamento.Id, request);

        Assert.Equal(StatusAgendamento.Cancelado, agendamentoCancelado.Status);
        Assert.Equal("Paciente solicitou cancelamento.", agendamentoCancelado.MotivoCancelamento);
    }

    [Fact]
    public void Cancelar_NaoDevePermitirMotivoVazio()
    {
        var contexto = CriarContexto();
        var agendamento = contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var request = new CancelarAgendamentoRequest("");

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.Cancelar(agendamento.Id, request));

        Assert.Equal("Motivo do cancelamento e obrigatorio.", exception.Message);
    }

    [Fact]
    public void Cancelar_NaoDevePermitirAgendamentoInexistente()
    {
        var contexto = CriarContexto();
        var request = new CancelarAgendamentoRequest("Paciente solicitou cancelamento.");

        var exception = Assert.Throws<ValidacaoException>(() =>
            contexto.Service.Cancelar(Guid.NewGuid(), request));

        Assert.Equal("Agendamento informado nao existe.", exception.Message);
    }

    [Fact]
    public void Cancelar_NaoDevePermitirCancelarDuasVezes()
    {
        var contexto = CriarContexto();
        var agendamento = contexto.Service.Criar(CriarRequest(contexto.Paciente.Id, contexto.Profissional.Id));
        var request = new CancelarAgendamentoRequest("Paciente solicitou cancelamento.");
        contexto.Service.Cancelar(agendamento.Id, request);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contexto.Service.Cancelar(agendamento.Id, request));

        Assert.Equal("Agendamento ja esta cancelado.", exception.Message);
    }

    private static CriarAgendamentoRequest CriarRequest(
        Guid pacienteId,
        Guid profissionalId)
    {
        return CriarRequest(
            pacienteId,
            profissionalId,
            new DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 7, 12, 9, 30, 0, TimeSpan.FromHours(-3)));
    }

    private static CriarAgendamentoRequest CriarRequest(
        Guid pacienteId,
        Guid profissionalId,
        DateTimeOffset inicio,
        DateTimeOffset fim)
    {
        return new CriarAgendamentoRequest(pacienteId, profissionalId, inicio, fim);
    }

    private static ContextoTeste CriarContexto()
    {
        var pacientes = new PacienteRepositorioFake();
        var profissionais = new ProfissionalRepositorioFake();
        var agendamentos = new AgendamentoRepositorioFake();
        var paciente = new Paciente(
            "Maria Silva",
            "12345678901",
            new DateOnly(1990, 5, 12),
            "11999990000");
        var profissional = new Profissional(
            "Dra Ana",
            PapelProfissional.Medico,
            "Clinica Geral",
            "CRM12345");
        pacientes.Adicionar(paciente);
        profissionais.Adicionar(profissional);
        var service = new AgendamentoService(agendamentos, pacientes, profissionais);

        return new ContextoTeste(service, pacientes, profissionais, agendamentos, paciente, profissional);
    }

    private sealed record ContextoTeste(
        AgendamentoService Service,
        PacienteRepositorioFake Pacientes,
        ProfissionalRepositorioFake Profissionais,
        AgendamentoRepositorioFake Agendamentos,
        Paciente Paciente,
        Profissional Profissional);

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
    }

    private sealed class PacienteRepositorioFake : IPacienteRepositorio
    {
        private readonly List<Paciente> pacientes = [];

        public IReadOnlyCollection<Paciente> Listar() => pacientes;

        public Paciente? ObterPorId(Guid id)
        {
            return pacientes.FirstOrDefault(paciente => paciente.Id == id);
        }

        public Paciente? ObterPorCpf(string cpf)
        {
            return pacientes.FirstOrDefault(paciente => paciente.Cpf == cpf);
        }

        public void Adicionar(Paciente paciente)
        {
            pacientes.Add(paciente);
        }
    }

    private sealed class ProfissionalRepositorioFake : IProfissionalRepositorio
    {
        private readonly List<Profissional> profissionais = [];

        public IReadOnlyCollection<Profissional> Listar() => profissionais;

        public Profissional? ObterPorId(Guid id)
        {
            return profissionais.FirstOrDefault(profissional => profissional.Id == id);
        }

        public Profissional? ObterPorRegistroProfissional(string registroProfissional)
        {
            return profissionais.FirstOrDefault(profissional =>
                profissional.RegistroProfissional == registroProfissional);
        }

        public void Adicionar(Profissional profissional)
        {
            profissionais.Add(profissional);
        }
    }
}
