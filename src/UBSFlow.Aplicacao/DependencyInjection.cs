using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;

namespace UBSFlow.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PacienteService>();
        services.AddScoped<ProfissionalService>();
        services.AddScoped<AgendamentoService>();

        return services;
    }
}
