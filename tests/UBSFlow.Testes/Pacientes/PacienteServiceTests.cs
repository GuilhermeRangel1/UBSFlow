using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Dominio.Pacientes;
using Xunit;

namespace UBSFlow.Testes.Pacientes;

public class PacienteServiceTests
{
    [Fact]
    public void Criar_DeveCadastrarPaciente()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        var request = new CriarPacienteRequest(
            "Maria Silva",
            "12345678901",
            new DateOnly(1990, 5, 12),
            "11999990000",
            null);

        var paciente = service.Criar(request);

        Assert.Equal("Maria Silva", paciente.Nome);
        Assert.Single(repositorio.Listar());
    }

    [Fact]
    public void Criar_NaoDevePermitirCpfDuplicado()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        var request = new CriarPacienteRequest(
            "Maria Silva",
            "12345678901",
            new DateOnly(1990, 5, 12),
            "11999990000",
            null);

        service.Criar(request);

        var exception = Assert.Throws<InvalidOperationException>(() => service.Criar(request));
        Assert.Equal("Ja existe um paciente cadastrado com este CPF.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirNomeVazio()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        var request = new CriarPacienteRequest(
            "",
            "12345678901",
            new DateOnly(1990, 5, 12),
            "11999990000",
            null);

        var exception = Assert.Throws<ValidacaoException>(() => service.Criar(request));
        Assert.Equal("Nome e obrigatorio.", exception.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234567890A")]
    public void Criar_NaoDevePermitirCpfInvalido(string cpf)
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        var request = new CriarPacienteRequest(
            "Maria Silva",
            cpf,
            new DateOnly(1990, 5, 12),
            "11999990000",
            null);

        var exception = Assert.Throws<ValidacaoException>(() => service.Criar(request));
        Assert.Equal("CPF deve conter exatamente 11 digitos.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirDataNascimentoNoFuturo()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        var request = new CriarPacienteRequest(
            "Maria Silva",
            "12345678901",
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            "11999990000",
            null);

        var exception = Assert.Throws<ValidacaoException>(() => service.Criar(request));
        Assert.Equal("Data de nascimento nao pode estar no futuro.", exception.Message);
    }

    [Fact]
    public void Listar_DeveFiltrarPorNome()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        service.Criar(CriarRequest("Maria Silva", "12345678901"));
        service.Criar(CriarRequest("Joao Santos", "98765432100"));

        var pacientes = service.Listar(new ListarPacientesRequest("maria", null));

        var paciente = Assert.Single(pacientes);
        Assert.Equal("Maria Silva", paciente.Nome);
    }

    [Fact]
    public void Listar_DeveFiltrarPorCpf()
    {
        var repositorio = new PacienteRepositorioFake();
        var service = new PacienteService(repositorio);
        service.Criar(CriarRequest("Maria Silva", "12345678901"));
        service.Criar(CriarRequest("Joao Santos", "98765432100"));

        var pacientes = service.Listar(new ListarPacientesRequest(null, "98765432100"));

        var paciente = Assert.Single(pacientes);
        Assert.Equal("Joao Santos", paciente.Nome);
    }

    private static CriarPacienteRequest CriarRequest(string nome, string cpf)
    {
        return new CriarPacienteRequest(
            nome,
            cpf,
            new DateOnly(1990, 5, 12),
            "11999990000",
            null);
    }

    private sealed class PacienteRepositorioFake : IPacienteRepositorio
    {
        private readonly List<Paciente> pacientes = [];

        public IReadOnlyCollection<Paciente> Listar()
        {
            return pacientes;
        }

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
}
