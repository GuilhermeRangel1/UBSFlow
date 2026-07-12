using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Infraestrutura.Pacientes;

namespace UBSFlow.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        services.AddSingleton<IPacienteRepositorio, PacienteRepositorioEmMemoria>();

        return services;
    }
}
