using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Agenda;

public class Agendamento : Entidade
{
    public Agendamento(
        Guid pacienteId,
        Guid profissionalId,
        DateTimeOffset inicio,
        DateTimeOffset fim)
    {
        PacienteId = pacienteId;
        ProfissionalId = profissionalId;
        Inicio = inicio;
        Fim = fim;
        Status = StatusAgendamento.Agendado;
    }

    public Guid PacienteId { get; private set; }
    public Guid ProfissionalId { get; private set; }
    public DateTimeOffset Inicio { get; private set; }
    public DateTimeOffset Fim { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public string? MotivoCancelamento { get; private set; }

    public void Remarcar(DateTimeOffset inicio, DateTimeOffset fim)
    {
        Inicio = inicio;
        Fim = fim;
        Status = StatusAgendamento.Remarcado;
        MarcarComoAtualizada();
    }

    public void Cancelar(string motivo)
    {
        MotivoCancelamento = motivo;
        Status = StatusAgendamento.Cancelado;
        MarcarComoAtualizada();
    }
}
