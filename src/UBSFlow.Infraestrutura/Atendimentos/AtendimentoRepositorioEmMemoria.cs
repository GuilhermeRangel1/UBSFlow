using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Dominio.Atendimentos;

namespace UBSFlow.Infraestrutura.Atendimentos;

public class AtendimentoRepositorioEmMemoria : IAtendimentoRepositorio
{
    private static readonly List<Atendimento> Atendimentos = [];

    public IReadOnlyCollection<Atendimento> Listar()
    {
        return Atendimentos;
    }

    public Atendimento? ObterPorId(Guid id)
    {
        return Atendimentos.FirstOrDefault(atendimento => atendimento.Id == id);
    }

    public Atendimento? ObterPorCheckInId(Guid checkInId)
    {
        return Atendimentos.FirstOrDefault(atendimento => atendimento.CheckInId == checkInId);
    }

    public void Adicionar(Atendimento atendimento)
    {
        Atendimentos.Add(atendimento);
    }
}
