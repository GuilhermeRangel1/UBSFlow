namespace UBSFlow.Aplicacao.Agenda;

public sealed record ListarAgendamentosRequest(
    Guid? PacienteId,
    Guid? ProfissionalId,
    DateOnly? Data,
    int Pagina = 1,
    int TamanhoPagina = 10);
