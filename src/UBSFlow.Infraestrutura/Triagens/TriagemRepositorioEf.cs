using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Triagens;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Triagens;

public class TriagemRepositorioEf : ITriagemRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public TriagemRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<Triagem> Listar()
    {
        return dbContext
            .Triagens
            .AsNoTracking()
            .OrderBy(triagem => triagem.RealizadaEm)
            .ToList();
    }

    public Triagem? ObterPorId(Guid id)
    {
        return dbContext.Triagens.FirstOrDefault(triagem => triagem.Id == id);
    }

    public Triagem? ObterPorCheckInId(Guid checkInId)
    {
        return dbContext.Triagens.FirstOrDefault(triagem => triagem.CheckInId == checkInId);
    }

    public void Adicionar(Triagem triagem)
    {
        dbContext.Triagens.Add(triagem);
        dbContext.SaveChanges();
    }
}
