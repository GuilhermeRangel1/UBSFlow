using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Pacientes;

namespace UBSFlow.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PacienteService>();

        return services;
    }
}
