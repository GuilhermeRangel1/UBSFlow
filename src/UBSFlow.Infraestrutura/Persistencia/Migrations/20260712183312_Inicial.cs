using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UBSFlow.Infraestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Inicio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Fim = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MotivoCancelamento = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "atendimentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Queixa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HipoteseDiagnostica = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Conduta = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Prescricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Encaminhamento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IniciadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinalizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atendimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "check_ins_atendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgendamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "uuid", nullable: false),
                    RealizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_ins_atendimento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "logs_auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Acao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Entidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    EntidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Usuario = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RegistradoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_auditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Cns = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "profissionais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Papel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Especialidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegistroProfissional = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profissionais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "triagens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Temperatura = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: false),
                    PressaoSistolica = table.Column<int>(type: "integer", nullable: false),
                    PressaoDiastolica = table.Column<int>(type: "integer", nullable: false),
                    FrequenciaCardiaca = table.Column<int>(type: "integer", nullable: false),
                    Sintomas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ClassificacaoRisco = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RealizadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_triagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "profissionais_disponibilidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiaSemana = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HoraFim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profissionais_disponibilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_profissionais_disponibilidades_profissionais_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_ProfissionalId_Inicio_Fim",
                table: "agendamentos",
                columns: new[] { "ProfissionalId", "Inicio", "Fim" });

            migrationBuilder.CreateIndex(
                name: "IX_atendimentos_CheckInId",
                table: "atendimentos",
                column: "CheckInId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_ins_atendimento_AgendamentoId",
                table: "check_ins_atendimento",
                column: "AgendamentoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_RegistradoEm",
                table: "logs_auditoria",
                column: "RegistradoEm");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_Cpf",
                table: "pacientes",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profissionais_disponibilidades_ProfissionalId",
                table: "profissionais_disponibilidades",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_triagens_CheckInId",
                table: "triagens",
                column: "CheckInId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendamentos");

            migrationBuilder.DropTable(
                name: "atendimentos");

            migrationBuilder.DropTable(
                name: "check_ins_atendimento");

            migrationBuilder.DropTable(
                name: "logs_auditoria");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "profissionais_disponibilidades");

            migrationBuilder.DropTable(
                name: "triagens");

            migrationBuilder.DropTable(
                name: "profissionais");
        }
    }
}
