using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NebuloHub.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAvaliacaoStartup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AVALIACAO_STARTUP_CNPJ",
                table: "AVALIACAO");

            migrationBuilder.DropForeignKey(
                name: "FK_AVALIACAO_STARTUP_StartupCNPJ1",
                table: "AVALIACAO");

            migrationBuilder.DropIndex(
                name: "IX_AVALIACAO_StartupCNPJ1",
                table: "AVALIACAO");

            migrationBuilder.DropColumn(
                name: "StartupCNPJ1",
                table: "AVALIACAO");

            migrationBuilder.RenameColumn(
                name: "CNPJ",
                table: "AVALIACAO",
                newName: "STARTUP_CNPJ");

            migrationBuilder.RenameIndex(
                name: "IX_AVALIACAO_CNPJ",
                table: "AVALIACAO",
                newName: "IX_AVALIACAO_STARTUP_CNPJ");

            migrationBuilder.AddForeignKey(
                name: "FK_AVALIACAO_STARTUP_STARTUP_CNPJ",
                table: "AVALIACAO",
                column: "STARTUP_CNPJ",
                principalTable: "STARTUP",
                principalColumn: "CNPJ",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AVALIACAO_STARTUP_STARTUP_CNPJ",
                table: "AVALIACAO");

            migrationBuilder.RenameColumn(
                name: "STARTUP_CNPJ",
                table: "AVALIACAO",
                newName: "CNPJ");

            migrationBuilder.RenameIndex(
                name: "IX_AVALIACAO_STARTUP_CNPJ",
                table: "AVALIACAO",
                newName: "IX_AVALIACAO_CNPJ");

            migrationBuilder.AddColumn<string>(
                name: "StartupCNPJ1",
                table: "AVALIACAO",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AVALIACAO_StartupCNPJ1",
                table: "AVALIACAO",
                column: "StartupCNPJ1");

            migrationBuilder.AddForeignKey(
                name: "FK_AVALIACAO_STARTUP_CNPJ",
                table: "AVALIACAO",
                column: "CNPJ",
                principalTable: "STARTUP",
                principalColumn: "CNPJ",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AVALIACAO_STARTUP_StartupCNPJ1",
                table: "AVALIACAO",
                column: "StartupCNPJ1",
                principalTable: "STARTUP",
                principalColumn: "CNPJ");
        }
    }
}
