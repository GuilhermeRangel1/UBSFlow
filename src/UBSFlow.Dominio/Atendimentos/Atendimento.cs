using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Atendimentos;

public class Atendimento : Entidade
{
    private Atendimento()
    {
        Queixa = string.Empty;
        HipoteseDiagnostica = string.Empty;
        Conduta = string.Empty;
    }

    public Atendimento(
        Guid checkInId,
        Guid pacienteId,
        Guid profissionalId,
        string queixa,
        string hipoteseDiagnostica,
        string conduta,
        string? prescricao,
        string? encaminhamento,
        DateTimeOffset iniciadoEm)
    {
        CheckInId = checkInId;
        PacienteId = pacienteId;
        ProfissionalId = profissionalId;
        Queixa = queixa;
        HipoteseDiagnostica = hipoteseDiagnostica;
        Conduta = conduta;
        Prescricao = prescricao;
        Encaminhamento = encaminhamento;
        IniciadoEm = iniciadoEm;
    }

    public Guid CheckInId { get; private set; }
    public Guid PacienteId { get; private set; }
    public Guid ProfissionalId { get; private set; }
    public string Queixa { get; private set; }
    public string HipoteseDiagnostica { get; private set; }
    public string Conduta { get; private set; }
    public string? Prescricao { get; private set; }
    public string? Encaminhamento { get; private set; }
    public DateTimeOffset IniciadoEm { get; private set; }
    public DateTimeOffset? FinalizadoEm { get; private set; }

    public void Finalizar(DateTimeOffset finalizadoEm)
    {
        FinalizadoEm = finalizadoEm;
        MarcarComoAtualizada();
    }
}
