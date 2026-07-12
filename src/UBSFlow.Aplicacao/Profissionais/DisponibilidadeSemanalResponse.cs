namespace UBSFlow.Aplicacao.Profissionais;

public sealed record DisponibilidadeSemanalResponse(
    DayOfWeek DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim);
