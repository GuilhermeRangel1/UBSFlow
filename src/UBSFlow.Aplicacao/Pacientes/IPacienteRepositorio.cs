using UBSFlow.Dominio.Pacientes;

namespace UBSFlow.Aplicacao.Pacientes;

public interface IPacienteRepositorio
{
    IReadOnlyCollection<Paciente> Listar();
    Paciente? ObterPorId(Guid id);
    Paciente? ObterPorCpf(string cpf);
    void Adicionar(Paciente paciente);
}
