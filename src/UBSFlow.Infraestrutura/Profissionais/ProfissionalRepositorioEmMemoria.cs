using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Infraestrutura.Profissionais;

public class ProfissionalRepositorioEmMemoria : IProfissionalRepositorio
{
    private static readonly List<Profissional> Profissionais = [];

    public IReadOnlyCollection<Profissional> Listar()
    {
        return Profissionais;
    }

    public Profissional? ObterPorId(Guid id)
    {
        return Profissionais.FirstOrDefault(profissional => profissional.Id == id);
    }

    public Profissional? ObterPorRegistroProfissional(string registroProfissional)
    {
        return Profissionais.FirstOrDefault(profissional =>
            profissional.RegistroProfissional == registroProfissional);
    }

    public void Adicionar(Profissional profissional)
    {
        Profissionais.Add(profissional);
    }
}
