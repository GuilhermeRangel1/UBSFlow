using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Relatorios;

public class RelatorioService
{
    private readonly IAtendimentoRepositorio atendimentoRepositorio;
    private readonly ITriagemRepositorio triagemRepositorio;
    private readonly IAgendamentoRepositorio agendamentoRepositorio;

    public RelatorioService(
        IAtendimentoRepositorio atendimentoRepositorio,
        ITriagemRepositorio triagemRepositorio,
        IAgendamentoRepositorio agendamentoRepositorio)
    {
        this.atendimentoRepositorio = atendimentoRepositorio;
        this.triagemRepositorio = triagemRepositorio;
        this.agendamentoRepositorio = agendamentoRepositorio;
    }

    public AtendimentosPorPeriodoResponse ObterAtendimentosPorPeriodo(RelatorioPeriodoRequest request)
    {
        ValidarPeriodo(request);

        var atendimentos = atendimentoRepositorio
            .Listar()
            .Where(atendimento => EstaNoPeriodo(atendimento.IniciadoEm, request))
            .ToList();

        return new AtendimentosPorPeriodoResponse(
            request.Inicio,
            request.Fim,
            atendimentos.Count,
            atendimentos.Count(atendimento => atendimento.FinalizadoEm is not null));
    }

    public ClassificacoesRiscoResponse ObterClassificacoesRisco(RelatorioPeriodoRequest request)
    {
        ValidarPeriodo(request);

        var itens = triagemRepositorio
            .Listar()
            .Where(triagem => EstaNoPeriodo(triagem.RealizadaEm, request))
            .GroupBy(triagem => triagem.ClassificacaoRisco)
            .Select(grupo => new ClassificacaoRiscoItemResponse(grupo.Key, grupo.Count()))
            .OrderBy(item => item.ClassificacaoRisco)
            .ToList();

        return new ClassificacoesRiscoResponse(request.Inicio, request.Fim, itens);
    }

    public CancelamentosResponse ObterCancelamentos(RelatorioPeriodoRequest request)
    {
        ValidarPeriodo(request);

        var total = agendamentoRepositorio
            .Listar()
            .Count(agendamento =>
                agendamento.Status == StatusAgendamento.Cancelado &&
                EstaNoPeriodo(agendamento.Inicio, request));

        return new CancelamentosResponse(request.Inicio, request.Fim, total);
    }

    private static void ValidarPeriodo(RelatorioPeriodoRequest request)
    {
        if (request.Fim < request.Inicio)
        {
            throw new ValidacaoException("Data final nao pode ser anterior a data inicial.");
        }
    }

    private static bool EstaNoPeriodo(DateTimeOffset dataHora, RelatorioPeriodoRequest request)
    {
        var data = DateOnly.FromDateTime(dataHora.LocalDateTime);

        return data >= request.Inicio && data <= request.Fim;
    }
}
