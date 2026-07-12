using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Triagens;

public interface ITriagemRepositorio
{
    IReadOnlyCollection<Triagem> Listar();
    Triagem? ObterPorId(Guid id);
    Triagem? ObterPorCheckInId(Guid checkInId);
    void Adicionar(Triagem triagem);
}
