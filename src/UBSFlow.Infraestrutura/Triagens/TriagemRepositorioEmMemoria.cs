using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Infraestrutura.Triagens;

public class TriagemRepositorioEmMemoria : ITriagemRepositorio
{
    private static readonly List<Triagem> Triagens = [];

    public IReadOnlyCollection<Triagem> Listar()
    {
        return Triagens;
    }

    public Triagem? ObterPorId(Guid id)
    {
        return Triagens.FirstOrDefault(triagem => triagem.Id == id);
    }

    public Triagem? ObterPorCheckInId(Guid checkInId)
    {
        return Triagens.FirstOrDefault(triagem => triagem.CheckInId == checkInId);
    }

    public void Adicionar(Triagem triagem)
    {
        Triagens.Add(triagem);
    }
}
