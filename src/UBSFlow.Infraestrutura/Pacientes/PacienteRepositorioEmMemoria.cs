using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Dominio.Pacientes;

namespace UBSFlow.Infraestrutura.Pacientes;

public class PacienteRepositorioEmMemoria : IPacienteRepositorio
{
    private static readonly List<Paciente> Pacientes = [];

    public IReadOnlyCollection<Paciente> Listar()
    {
        return Pacientes;
    }

    public Paciente? ObterPorId(Guid id)
    {
        return Pacientes.FirstOrDefault(paciente => paciente.Id == id);
    }

    public Paciente? ObterPorCpf(string cpf)
    {
        return Pacientes.FirstOrDefault(paciente => paciente.Cpf == cpf);
    }

    public void Adicionar(Paciente paciente)
    {
        Pacientes.Add(paciente);
    }
}
