using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Agenda;

public class AgendamentoRepositorioEf : IAgendamentoRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public AgendamentoRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<Agendamento> Listar()
    {
        return dbContext
            .Agendamentos
            .AsNoTracking()
            .OrderBy(agendamento => agendamento.Inicio)
            .ToList();
    }

    public Agendamento? ObterPorId(Guid id)
    {
        return dbContext.Agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
    }

    public void Adicionar(Agendamento agendamento)
    {
        dbContext.Agendamentos.Add(agendamento);
        dbContext.SaveChanges();
    }

    public void SalvarAlteracoes()
    {
        dbContext.SaveChanges();
    }
}
