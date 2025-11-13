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
                    ID_HABILIDADE = table.Column<long>(type: "NUMBER(19)", nullable: false, defaultValueSql: "HABILIDADE_SEQ.NEXTVAL"),
                    NOME_HABILIDADE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TIPO_HABILIDADE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HABILIDADE", x => x.ID_HABILIDADE);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    CPF = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ROLE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TELEFONE = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.CPF);
                });

            migrationBuilder.CreateTable(
                name: "STARTUP",
                columns: table => new
                {
                    CNPJ = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    VIDEO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NOME_STARTUP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SITE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NOME_RESPONSAVEL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EMAIL_STARTUP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    ID_AVALIACAO = table.Column<long>(type: "NUMBER(19)", nullable: false, defaultValueSql: "AVALIACAO_SEQ.NEXTVAL"),
                    NOTA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    COMENTARIO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CPF = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVALIACAO", x => x.ID_AVALIACAO);
                    table.ForeignKey(
                        name: "FK_AVALIACAO_STARTUP_CNPJ",
                        column: x => x.CNPJ,
                        principalTable: "STARTUP",
                        principalColumn: "CNPJ",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AVALIACAO_USUARIO_CPF",
                        column: x => x.CPF,
                        principalTable: "USUARIO",
                        principalColumn: "CPF",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POSSUI",
                columns: table => new
                {
                    ID_POSSUI = table.Column<long>(type: "NUMBER(19)", nullable: false, defaultValueSql: "POSSUI_SEQ.NEXTVAL"),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ID_HABILIDADE = table.Column<long>(type: "NUMBER(19)", nullable: false)
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
