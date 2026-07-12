using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UBSFlow.Infraestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosAutenticacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Login = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Papel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "Login", "Nome", "Papel", "SenhaHash" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), true, null, new DateTimeOffset(new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin", "Administrador", "ADMIN", "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, null, new DateTimeOffset(new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "recepcao", "Recepcao UBS", "RECEPCIONISTA", "2b2f7d1f89222d7211f2befb172fa7267c44ef66f9e074a7902dc39ce7c829a2" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), true, null, new DateTimeOffset(new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "enfermagem", "Enfermagem UBS", "ENFERMEIRO", "354e5812214dc445b5f82fd0183b7f0fb0eff7bd769d5dfff724f37db0d9b010" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), true, null, new DateTimeOffset(new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "medico", "Medico UBS", "MEDICO", "673ab82a6530ee3bd9b04ee72a4d66afa7fa059aedc685cf44e35d29d90ebafa" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), true, null, new DateTimeOffset(new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "gestao", "Gestao UBS", "GESTOR", "68c0bffdcdb90414371ae21018e04c9cb56d296a23309ccbd7717fc9e41a33b0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Login",
                table: "usuarios",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
