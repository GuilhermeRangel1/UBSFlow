using UBSFlow.Dominio.Fila;

namespace UBSFlow.Aplicacao.Fila;

public interface ICheckInAtendimentoRepositorio
{
    IReadOnlyCollection<CheckInAtendimento> Listar();
    CheckInAtendimento? ObterPorAgendamentoId(Guid agendamentoId);
    void Adicionar(CheckInAtendimento checkIn);
}
