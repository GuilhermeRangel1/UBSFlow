namespace UBSFlow.Aplicacao.Profissionais;

public sealed record DisponibilidadeSemanalRequest(
    DayOfWeek DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim);
