using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Atendimentos;

public class AtendimentoRepositorioEf : IAtendimentoRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public AtendimentoRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<Atendimento> Listar()
    {
        return dbContext
            .Atendimentos
            .AsNoTracking()
            .OrderBy(atendimento => atendimento.IniciadoEm)
            .ToList();
    }

    public Atendimento? ObterPorId(Guid id)
    {
        return dbContext.Atendimentos.FirstOrDefault(atendimento => atendimento.Id == id);
    }

    public Atendimento? ObterPorCheckInId(Guid checkInId)
    {
        return dbContext.Atendimentos.FirstOrDefault(atendimento => atendimento.CheckInId == checkInId);
    }

    public void Adicionar(Atendimento atendimento)
    {
        dbContext.Atendimentos.Add(atendimento);
        dbContext.SaveChanges();
    }

    public void SalvarAlteracoes()
    {
        dbContext.SaveChanges();
    }
}
