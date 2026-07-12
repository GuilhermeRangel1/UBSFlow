using Microsoft.EntityFrameworkCore;
using UBSFlow.Dominio.Agenda;
using UBSFlow.Dominio.Atendimentos;
using UBSFlow.Dominio.Auditoria;
using UBSFlow.Dominio.Fila;
using UBSFlow.Dominio.Pacientes;
using UBSFlow.Dominio.Profissionais;
using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Infraestrutura.Persistencia;

public class UbsFlowDbContext : DbContext
{
    public UbsFlowDbContext(DbContextOptions<UbsFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Profissional> Profissionais => Set<Profissional>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<CheckInAtendimento> CheckInsAtendimento => Set<CheckInAtendimento>();
    public DbSet<Triagem> Triagens => Set<Triagem>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarPacientes(modelBuilder);
        ConfigurarProfissionais(modelBuilder);
        ConfigurarAgendamentos(modelBuilder);
        ConfigurarCheckIns(modelBuilder);
        ConfigurarTriagens(modelBuilder);
        ConfigurarAtendimentos(modelBuilder);
        ConfigurarAuditoria(modelBuilder);
    }

    private static void ConfigurarPacientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("pacientes");
            entity.HasKey(paciente => paciente.Id);
            entity.Property(paciente => paciente.Nome).HasMaxLength(160).IsRequired();
            entity.Property(paciente => paciente.Cpf).HasMaxLength(11).IsRequired();
            entity.Property(paciente => paciente.Cns).HasMaxLength(15);
            entity.Property(paciente => paciente.Telefone).HasMaxLength(30).IsRequired();
            entity.HasIndex(paciente => paciente.Cpf).IsUnique();
        });
    }

    private static void ConfigurarProfissionais(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profissional>(entity =>
        {
            entity.ToTable("profissionais");
            entity.HasKey(profissional => profissional.Id);
            entity.Property(profissional => profissional.Nome).HasMaxLength(160).IsRequired();
            entity.Property(profissional => profissional.Papel).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(profissional => profissional.Especialidade).HasMaxLength(100);
            entity.Property(profissional => profissional.RegistroProfissional).HasMaxLength(30);

            entity.OwnsMany(profissional => profissional.Disponibilidades, disponibilidade =>
            {
                disponibilidade.ToTable("profissionais_disponibilidades");
                disponibilidade.WithOwner().HasForeignKey("ProfissionalId");
                disponibilidade.Property<Guid>("Id");
                disponibilidade.HasKey("Id");
                disponibilidade.Property(item => item.DiaSemana).HasConversion<string>().HasMaxLength(20).IsRequired();
                disponibilidade.Property(item => item.HoraInicio).IsRequired();
                disponibilidade.Property(item => item.HoraFim).IsRequired();
            });
        });
    }

    private static void ConfigurarAgendamentos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.ToTable("agendamentos");
            entity.HasKey(agendamento => agendamento.Id);
            entity.Property(agendamento => agendamento.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(agendamento => agendamento.MotivoCancelamento).HasMaxLength(300);
            entity.HasIndex(agendamento => new { agendamento.ProfissionalId, agendamento.Inicio, agendamento.Fim });
        });
    }

    private static void ConfigurarCheckIns(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckInAtendimento>(entity =>
        {
            entity.ToTable("check_ins_atendimento");
            entity.HasKey(checkIn => checkIn.Id);
            entity.Property(checkIn => checkIn.Status).HasConversion<string>().HasMaxLength(40).IsRequired();
            entity.HasIndex(checkIn => checkIn.AgendamentoId).IsUnique();
        });
    }

    private static void ConfigurarTriagens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Triagem>(entity =>
        {
            entity.ToTable("triagens");
            entity.HasKey(triagem => triagem.Id);
            entity.Property(triagem => triagem.Temperatura).HasPrecision(4, 1).IsRequired();
            entity.Property(triagem => triagem.Sintomas).HasMaxLength(500).IsRequired();
            entity.Property(triagem => triagem.ClassificacaoRisco).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(triagem => triagem.Observacoes).HasMaxLength(1000);
            entity.HasIndex(triagem => triagem.CheckInId).IsUnique();
        });
    }

    private static void ConfigurarAtendimentos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atendimento>(entity =>
        {
            entity.ToTable("atendimentos");
            entity.HasKey(atendimento => atendimento.Id);
            entity.Property(atendimento => atendimento.Queixa).HasMaxLength(500).IsRequired();
            entity.Property(atendimento => atendimento.HipoteseDiagnostica).HasMaxLength(500).IsRequired();
            entity.Property(atendimento => atendimento.Conduta).HasMaxLength(1000).IsRequired();
            entity.Property(atendimento => atendimento.Prescricao).HasMaxLength(1000);
            entity.Property(atendimento => atendimento.Encaminhamento).HasMaxLength(500);
            entity.HasIndex(atendimento => atendimento.CheckInId).IsUnique();
        });
    }

    private static void ConfigurarAuditoria(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogAuditoria>(entity =>
        {
            entity.ToTable("logs_auditoria");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.Acao).HasConversion<string>().HasMaxLength(60).IsRequired();
            entity.Property(log => log.Entidade).HasMaxLength(80).IsRequired();
            entity.Property(log => log.Usuario).HasMaxLength(120).IsRequired();
            entity.Property(log => log.Descricao).HasMaxLength(1000).IsRequired();
            entity.HasIndex(log => log.RegistradoEm);
        });
    }
}
