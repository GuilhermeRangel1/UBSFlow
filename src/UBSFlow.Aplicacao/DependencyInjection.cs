using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Aplicacao.Triagens;

namespace UBSFlow.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PacienteService>();
        services.AddScoped<ProfissionalService>();
        services.AddScoped<AgendamentoService>();
        services.AddScoped<FilaAtendimentoService>();
        services.AddScoped<TriagemService>();

        return services;
    }
}
