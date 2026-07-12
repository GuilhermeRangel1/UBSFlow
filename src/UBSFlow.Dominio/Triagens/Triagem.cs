using UBSFlow.Dominio.Comum;

namespace UBSFlow.Dominio.Triagens;

public class Triagem : Entidade
{
    public Triagem(
        Guid checkInId,
        Guid pacienteId,
        decimal temperatura,
        int pressaoSistolica,
        int pressaoDiastolica,
        int frequenciaCardiaca,
        string sintomas,
        ClassificacaoRisco classificacaoRisco,
        string? observacoes,
        DateTimeOffset realizadaEm)
    {
        CheckInId = checkInId;
        PacienteId = pacienteId;
        Temperatura = temperatura;
        PressaoSistolica = pressaoSistolica;
        PressaoDiastolica = pressaoDiastolica;
        FrequenciaCardiaca = frequenciaCardiaca;
        Sintomas = sintomas;
        ClassificacaoRisco = classificacaoRisco;
        Observacoes = observacoes;
        RealizadaEm = realizadaEm;
    }

    public Guid CheckInId { get; private set; }
    public Guid PacienteId { get; private set; }
    public decimal Temperatura { get; private set; }
    public int PressaoSistolica { get; private set; }
    public int PressaoDiastolica { get; private set; }
    public int FrequenciaCardiaca { get; private set; }
    public string Sintomas { get; private set; }
    public ClassificacaoRisco ClassificacaoRisco { get; private set; }
    public string? Observacoes { get; private set; }
    public DateTimeOffset RealizadaEm { get; private set; }
}
