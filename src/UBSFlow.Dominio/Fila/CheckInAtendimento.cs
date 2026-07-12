using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Fila;

public class CheckInAtendimento : Entidade
{
    public CheckInAtendimento(
        Guid agendamentoId,
        Guid pacienteId,
        Guid profissionalId,
        DateTimeOffset realizadoEm)
    {
        AgendamentoId = agendamentoId;
        PacienteId = pacienteId;
        ProfissionalId = profissionalId;
        RealizadoEm = realizadoEm;
        Status = StatusFilaAtendimento.AguardandoTriagem;
    }

    public Guid AgendamentoId { get; private set; }
    public Guid PacienteId { get; private set; }
    public Guid ProfissionalId { get; private set; }
    public DateTimeOffset RealizadoEm { get; private set; }
    public StatusFilaAtendimento Status { get; private set; }
}
