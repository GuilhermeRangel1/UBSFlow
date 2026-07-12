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
