using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Pacientes;

public class Paciente : Entidade
{
    public Paciente(
        string nome,
        string cpf,
        DateOnly dataNascimento,
        string telefone,
        string? cns = null)
    {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Telefone = telefone;
        Cns = cns;
    }

    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public string? Cns { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    public string Telefone { get; private set; }
}
