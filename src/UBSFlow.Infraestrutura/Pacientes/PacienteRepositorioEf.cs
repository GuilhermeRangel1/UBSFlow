using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Dominio.Pacientes;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Pacientes;

public class PacienteRepositorioEf : IPacienteRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public PacienteRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<Paciente> Listar()
    {
        return dbContext
            .Pacientes
            .AsNoTracking()
            .OrderBy(paciente => paciente.Nome)
            .ToList();
    }

    public Paciente? ObterPorId(Guid id)
    {
        return dbContext.Pacientes.FirstOrDefault(paciente => paciente.Id == id);
    }

    public Paciente? ObterPorCpf(string cpf)
    {
        return dbContext.Pacientes.FirstOrDefault(paciente => paciente.Cpf == cpf);
    }

    public void Adicionar(Paciente paciente)
    {
        dbContext.Pacientes.Add(paciente);
        dbContext.SaveChanges();
    }
}
