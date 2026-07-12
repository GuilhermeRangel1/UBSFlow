using UBSFlow.Dominio.Atendimentos;

namespace UBSFlow.Aplicacao.Atendimentos;

public interface IAtendimentoRepositorio
{
    IReadOnlyCollection<Atendimento> Listar();
    Atendimento? ObterPorId(Guid id);
    Atendimento? ObterPorCheckInId(Guid checkInId);
    void Adicionar(Atendimento atendimento);
    void SalvarAlteracoes();
}
