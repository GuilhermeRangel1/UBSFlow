using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Dominio.Fila;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Fila;

public class CheckInAtendimentoRepositorioEf : ICheckInAtendimentoRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public CheckInAtendimentoRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<CheckInAtendimento> Listar()
    {
        return dbContext
            .CheckInsAtendimento
            .AsNoTracking()
            .OrderBy(checkIn => checkIn.RealizadoEm)
            .ToList();
    }

    public CheckInAtendimento? ObterPorId(Guid id)
    {
        return dbContext.CheckInsAtendimento.FirstOrDefault(checkIn => checkIn.Id == id);
    }

    public CheckInAtendimento? ObterPorAgendamentoId(Guid agendamentoId)
    {
        return dbContext.CheckInsAtendimento.FirstOrDefault(checkIn => checkIn.AgendamentoId == agendamentoId);
    }

    public void Adicionar(CheckInAtendimento checkIn)
    {
        dbContext.CheckInsAtendimento.Add(checkIn);
        dbContext.SaveChanges();
    }

    public void SalvarAlteracoes()
    {
        dbContext.SaveChanges();
    }
}
