using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Aplicacao.Profissionais;

public interface IProfissionalRepositorio
{
    IReadOnlyCollection<Profissional> Listar();
    Profissional? ObterPorId(Guid id);
    Profissional? ObterPorRegistroProfissional(string registroProfissional);
    void Adicionar(Profissional profissional);
}
