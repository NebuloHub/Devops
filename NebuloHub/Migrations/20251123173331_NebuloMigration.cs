using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NebuloHub.Migrations
{
    /// <inheritdoc />
    public partial class NebuloMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HABILIDADE",
                columns: table => new
                {
                    ID_HABILIDADE = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOME_HABILIDADE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TIPO_HABILIDADE = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HABILIDADE", x => x.ID_HABILIDADE);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    CPF = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NOME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SENHA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ROLE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TELEFONE = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.CPF);
                });

            migrationBuilder.CreateTable(
                name: "STARTUP",
                columns: table => new
                {
                    CNPJ = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VIDEO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NOME_STARTUP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SITE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DESCRICAO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NOME_RESPONSAVEL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL_STARTUP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STARTUP", x => x.CNPJ);
                    table.ForeignKey(
                        name: "FK_STARTUP_USUARIO_CPF",
                        column: x => x.CPF,
                        principalTable: "USUARIO",
                        principalColumn: "CPF",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AVALIACAO",
                columns: table => new
                {
                    ID_AVALIACAO = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOTA = table.Column<long>(type: "bigint", nullable: false),
                    COMENTARIO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartupCNPJ1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVALIACAO", x => x.ID_AVALIACAO);
                    table.ForeignKey(
                        name: "FK_AVALIACAO_STARTUP_CNPJ",
                        column: x => x.CNPJ,
                        principalTable: "STARTUP",
                        principalColumn: "CNPJ",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AVALIACAO_STARTUP_StartupCNPJ1",
                        column: x => x.StartupCNPJ1,
                        principalTable: "STARTUP",
                        principalColumn: "CNPJ");
                    table.ForeignKey(
                        name: "FK_AVALIACAO_USUARIO_CPF",
                        column: x => x.CPF,
                        principalTable: "USUARIO",
                        principalColumn: "CPF",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSSUI",
                columns: table => new
                {
                    ID_POSSUI = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CNPJ = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID_HABILIDADE = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSSUI", x => x.ID_POSSUI);
                    table.ForeignKey(
                        name: "CNPJ_POSSUI_FK",
                        column: x => x.CNPJ,
                        principalTable: "STARTUP",
                        principalColumn: "CNPJ",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ID_HABILIDADE_POSSUI_FK",
                        column: x => x.ID_HABILIDADE,
                        principalTable: "HABILIDADE",
                        principalColumn: "ID_HABILIDADE",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AVALIACAO_CNPJ",
                table: "AVALIACAO",
                column: "CNPJ");

            migrationBuilder.CreateIndex(
                name: "IX_AVALIACAO_CPF",
                table: "AVALIACAO",
                column: "CPF");

            migrationBuilder.CreateIndex(
                name: "IX_AVALIACAO_StartupCNPJ1",
                table: "AVALIACAO",
                column: "StartupCNPJ1");

            migrationBuilder.CreateIndex(
                name: "IX_POSSUI_CNPJ",
                table: "POSSUI",
                column: "CNPJ");

            migrationBuilder.CreateIndex(
                name: "IX_POSSUI_ID_HABILIDADE",
                table: "POSSUI",
                column: "ID_HABILIDADE");

            migrationBuilder.CreateIndex(
                name: "IX_STARTUP_CPF",
                table: "STARTUP",
                column: "CPF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AVALIACAO");

            migrationBuilder.DropTable(
                name: "POSSUI");

            migrationBuilder.DropTable(
                name: "STARTUP");

            migrationBuilder.DropTable(
                name: "HABILIDADE");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
