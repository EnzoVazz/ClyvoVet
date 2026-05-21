using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClyvoVet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    LOGRADOURO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLINICA", x => x.ID_CLINICA);
                });

            migrationBuilder.CreateTable(
                name: "PRONTUARIO",
                columns: table => new
                {
                    ID_PRONTUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DATA_REGISTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRONTUARIO", x => x.ID_PRONTUARIO);
                });

            migrationBuilder.CreateTable(
                name: "TUTOR",
                columns: table => new
                {
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUTOR", x => x.ID_TUTOR);
                });

            migrationBuilder.CreateTable(
                name: "VETERINARIO",
                columns: table => new
                {
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CRMV = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VETERINARIO", x => x.ID_VETERINARIO);
                });

            migrationBuilder.CreateTable(
                name: "PET",
                columns: table => new
                {
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    COR = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    IDADE = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    PESO = table.Column<decimal>(type: "NUMBER(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.ID_PET);
                    table.ForeignKey(
                        name: "FK_PET_TUTOR_ID_TUTOR",
                        column: x => x.ID_TUTOR,
                        principalTable: "TUTOR",
                        principalColumn: "ID_TUTOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_PRONTUARIO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DATA_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_CONSULTA_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PRONTUARIO_ID_PRONTUARIO",
                        column: x => x.ID_PRONTUARIO,
                        principalTable: "PRONTUARIO",
                        principalColumn: "ID_PRONTUARIO");
                    table.ForeignKey(
                        name: "FK_CONSULTA_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "CLINICA_CNPJ_UK",
                table: "CLINICA",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_CLINICA",
                table: "CONSULTA",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_PET",
                table: "CONSULTA",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_PRONTUARIO",
                table: "CONSULTA",
                column: "ID_PRONTUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_VETERINARIO",
                table: "CONSULTA",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_TUTOR",
                table: "PET",
                column: "ID_TUTOR");

            migrationBuilder.CreateIndex(
                name: "TUTOR_CPF_UK",
                table: "TUTOR",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "VETERINARIO_CRMV_UK",
                table: "VETERINARIO",
                column: "CRMV",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONSULTA");

            migrationBuilder.DropTable(
                name: "CLINICA");

            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "PRONTUARIO");

            migrationBuilder.DropTable(
                name: "VETERINARIO");

            migrationBuilder.DropTable(
                name: "TUTOR");
        }
    }
}
