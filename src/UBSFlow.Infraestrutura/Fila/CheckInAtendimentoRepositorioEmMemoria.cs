using UBSFlow.Aplicacao.Fila;
using UBSFlow.Dominio.Fila;

namespace UBSFlow.Infraestrutura.Fila;

public class CheckInAtendimentoRepositorioEmMemoria : ICheckInAtendimentoRepositorio
{
    private static readonly List<CheckInAtendimento> CheckIns = [];

    public IReadOnlyCollection<CheckInAtendimento> Listar()
    {
        return CheckIns;
    }

    public CheckInAtendimento? ObterPorId(Guid id)
    {
        return CheckIns.FirstOrDefault(checkIn => checkIn.Id == id);
    }

    public CheckInAtendimento? ObterPorAgendamentoId(Guid agendamentoId)
    {
        return CheckIns.FirstOrDefault(checkIn => checkIn.AgendamentoId == agendamentoId);
    }

    public void Adicionar(CheckInAtendimento checkIn)
    {
        CheckIns.Add(checkIn);
    }
}
