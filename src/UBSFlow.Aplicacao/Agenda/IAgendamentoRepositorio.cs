using UBSFlow.Dominio.Agenda;

namespace UBSFlow.Aplicacao.Agenda;

public interface IAgendamentoRepositorio
{
    IReadOnlyCollection<Agendamento> Listar();
    Agendamento? ObterPorId(Guid id);
    void Adicionar(Agendamento agendamento);
    void SalvarAlteracoes();
}
