using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Dominio.Agenda;

namespace UBSFlow.Infraestrutura.Agenda;

public class AgendamentoRepositorioEmMemoria : IAgendamentoRepositorio
{
    private static readonly List<Agendamento> Agendamentos = [];

    public IReadOnlyCollection<Agendamento> Listar()
    {
        return Agendamentos;
    }

    public Agendamento? ObterPorId(Guid id)
    {
        return Agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
    }

    public void Adicionar(Agendamento agendamento)
    {
        Agendamentos.Add(agendamento);
    }

    public void SalvarAlteracoes()
    {
    }
}
