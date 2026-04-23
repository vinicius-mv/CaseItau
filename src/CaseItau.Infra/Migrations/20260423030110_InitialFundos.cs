using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CaseItau.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialFundos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TIPO_FUNDO",
                columns: table => new
                {
                    CODIGO = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NOME = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_FUNDO", x => x.CODIGO);
                });

            migrationBuilder.CreateTable(
                name: "FUNDO",
                columns: table => new
                {
                    CODIGO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NOME = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    PATRIMONIO = table.Column<decimal>(type: "numeric", nullable: true),
                    CODIGO_TIPO = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FUNDO", x => x.CODIGO);
                    table.ForeignKey(
                        name: "FK_FUNDO_TIPO_FUNDO_CODIGO_TIPO",
                        column: x => x.CODIGO_TIPO,
                        principalTable: "TIPO_FUNDO",
                        principalColumn: "CODIGO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FUNDO_CNPJ",
                table: "FUNDO",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FUNDO_CODIGO_TIPO",
                table: "FUNDO",
                column: "CODIGO_TIPO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FUNDO");

            migrationBuilder.DropTable(
                name: "TIPO_FUNDO");
        }
    }
}
