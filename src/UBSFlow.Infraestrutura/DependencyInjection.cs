using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Aplicacao.Triagens;
using UBSFlow.Infraestrutura.Agenda;
using UBSFlow.Infraestrutura.Auditoria;
using UBSFlow.Infraestrutura.Atendimentos;
using UBSFlow.Infraestrutura.Fila;
using UBSFlow.Infraestrutura.Pacientes;
using UBSFlow.Infraestrutura.Persistencia;
using UBSFlow.Infraestrutura.Profissionais;
using UBSFlow.Infraestrutura.Triagens;

namespace UBSFlow.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UbsFlowDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPacienteRepositorio, PacienteRepositorioEf>();
        services.AddSingleton<IProfissionalRepositorio, ProfissionalRepositorioEmMemoria>();
        services.AddSingleton<IAgendamentoRepositorio, AgendamentoRepositorioEmMemoria>();
        services.AddSingleton<ICheckInAtendimentoRepositorio, CheckInAtendimentoRepositorioEmMemoria>();
        services.AddSingleton<ITriagemRepositorio, TriagemRepositorioEmMemoria>();
        services.AddSingleton<IAtendimentoRepositorio, AtendimentoRepositorioEmMemoria>();
        services.AddSingleton<IAuditoriaRepositorio, AuditoriaRepositorioEmMemoria>();

        return services;
    }
}
