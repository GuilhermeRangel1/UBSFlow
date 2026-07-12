using Microsoft.Extensions.DependencyInjection;
using UBSFlow.Aplicacao.Auditoria;
using UBSFlow.Aplicacao.Agenda;
using UBSFlow.Aplicacao.Atendimentos;
using UBSFlow.Aplicacao.Autenticacao;
using UBSFlow.Aplicacao.Fila;
using UBSFlow.Aplicacao.Pacientes;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Aplicacao.Relatorios;
using UBSFlow.Aplicacao.Triagens;

namespace UBSFlow.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PacienteService>();
        services.AddScoped<HistoricoPacienteService>();
        services.AddScoped<ProfissionalService>();
        services.AddScoped<AgendamentoService>();
        services.AddScoped<FilaAtendimentoService>();
        services.AddScoped<TriagemService>();
        services.AddScoped<AtendimentoService>();
        services.AddScoped<RelatorioService>();
        services.AddScoped<AuditoriaService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
