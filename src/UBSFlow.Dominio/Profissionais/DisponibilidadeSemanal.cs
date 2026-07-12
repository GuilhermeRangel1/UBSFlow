namespace UBSFlow.Dominio.Profissionais;

public class DisponibilidadeSemanal
{
    private DisponibilidadeSemanal()
    {
    }

    public DisponibilidadeSemanal(
        DayOfWeek diaSemana,
        TimeOnly horaInicio,
        TimeOnly horaFim)
    {
        DiaSemana = diaSemana;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    public DayOfWeek DiaSemana { get; private set; }
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFim { get; private set; }
}
