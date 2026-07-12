using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Pacientes;

public class HistoricoPacienteService
{
    private readonly IPacienteRepositorio pacienteRepositorio;
    private readonly IAgendamentoRepositorio agendamentoRepositorio;
    private readonly ICheckInAtendimentoRepositorio checkInRepositorio;
    private readonly ITriagemRepositorio triagemRepositorio;
    private readonly IAtendimentoRepositorio atendimentoRepositorio;

    public HistoricoPacienteService(
        IPacienteRepositorio pacienteRepositorio,
        IAgendamentoRepositorio agendamentoRepositorio,
        ICheckInAtendimentoRepositorio checkInRepositorio,
        ITriagemRepositorio triagemRepositorio,
        IAtendimentoRepositorio atendimentoRepositorio)
    {
        this.pacienteRepositorio = pacienteRepositorio;
        this.agendamentoRepositorio = agendamentoRepositorio;
        this.checkInRepositorio = checkInRepositorio;
        this.triagemRepositorio = triagemRepositorio;
        this.atendimentoRepositorio = atendimentoRepositorio;
    }

    public HistoricoPacienteResponse Obter(Guid pacienteId)
    {
        if (pacienteRepositorio.ObterPorId(pacienteId) is null)
        {
            throw new ValidacaoException("Paciente informado nao existe.");
        }

        var agendamentos = agendamentoRepositorio
            .Listar()
            .Where(agendamento => agendamento.PacienteId == pacienteId)
            .OrderByDescending(agendamento => agendamento.Inicio)
            .ToList();

        var checkInIds = checkInRepositorio
            .Listar()
            .Where(checkIn => checkIn.PacienteId == pacienteId)
            .Select(checkIn => checkIn.Id)
            .ToHashSet();

        var triagens = triagemRepositorio
            .Listar()
            .Where(triagem => triagem.PacienteId == pacienteId || checkInIds.Contains(triagem.CheckInId))
            .OrderByDescending(triagem => triagem.RealizadaEm)
            .ToList();

        var atendimentos = atendimentoRepositorio
            .Listar()
            .Where(atendimento => atendimento.PacienteId == pacienteId || checkInIds.Contains(atendimento.CheckInId))
            .OrderByDescending(atendimento => atendimento.IniciadoEm)
            .ToList();

        return new HistoricoPacienteResponse(
            pacienteId,
            agendamentos.Select(MapearAgendamento).ToList(),
            triagens.Select(MapearTriagem).ToList(),
            atendimentos.Select(MapearAtendimento).ToList());
    }

    private static AgendamentoHistoricoResponse MapearAgendamento(Agendamento agendamento)
    {
        return new AgendamentoHistoricoResponse(
            agendamento.Id,
            agendamento.ProfissionalId,
            agendamento.Inicio,
            agendamento.Fim,
            agendamento.Status);
    }

    private static TriagemHistoricoResponse MapearTriagem(Triagem triagem)
    {
        return new TriagemHistoricoResponse(
            triagem.Id,
            triagem.CheckInId,
            triagem.Temperatura,
            triagem.PressaoSistolica,
            triagem.PressaoDiastolica,
            triagem.FrequenciaCardiaca,
            triagem.Sintomas,
            triagem.ClassificacaoRisco,
            triagem.RealizadaEm);
    }

    private static AtendimentoHistoricoResponse MapearAtendimento(Atendimento atendimento)
    {
        return new AtendimentoHistoricoResponse(
            atendimento.Id,
            atendimento.CheckInId,
            atendimento.ProfissionalId,
            atendimento.Queixa,
            atendimento.HipoteseDiagnostica,
            atendimento.Conduta,
            atendimento.Prescricao,
            atendimento.Encaminhamento,
            atendimento.IniciadoEm,
            atendimento.FinalizadoEm);
    }
}
