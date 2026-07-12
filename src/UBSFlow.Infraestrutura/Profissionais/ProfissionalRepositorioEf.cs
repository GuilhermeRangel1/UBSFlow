using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Profissionais;
using UBSFlow.Infraestrutura.Persistencia;

namespace UBSFlow.Infraestrutura.Profissionais;

public class ProfissionalRepositorioEf : IProfissionalRepositorio
{
    private readonly UbsFlowDbContext dbContext;

    public ProfissionalRepositorioEf(UbsFlowDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IReadOnlyCollection<Profissional> Listar()
    {
        return dbContext
            .Profissionais
            .AsNoTracking()
            .OrderBy(profissional => profissional.Nome)
            .ToList();
    }

    public Profissional? ObterPorId(Guid id)
    {
        return dbContext.Profissionais.FirstOrDefault(profissional => profissional.Id == id);
    }

    public Profissional? ObterPorRegistroProfissional(string registroProfissional)
    {
        return dbContext.Profissionais.FirstOrDefault(profissional =>
            profissional.RegistroProfissional == registroProfissional);
    }

    public void Adicionar(Profissional profissional)
    {
        dbContext.Profissionais.Add(profissional);
        dbContext.SaveChanges();
    }
}
