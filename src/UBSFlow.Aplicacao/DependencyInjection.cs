using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;

namespace UBSFlow.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PacienteService>();
        services.AddScoped<ProfissionalService>();

        return services;
    }
}
