using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPIlist.Migrations
{
    public partial class inicialModelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Epis",
                columns: table => new
                {
                    EpiID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    C_A = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epis", x => x.EpiID);
                });

            migrationBuilder.CreateTable(
                name: "Unidades",
                columns: table => new
                {
                    UnidadeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.UnidadeID);
                });

            migrationBuilder.CreateTable(
                name: "Equipes",
                columns: table => new
                {
                    EquipeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeEquipe = table.Column<string>(type: "TEXT", nullable: false),
                    UnidadeID = table.Column<int>(type: "INTEGER", nullable: false),
                    LiderID = table.Column<int>(type: "INTEGER", nullable: true),
                    LiderUsuarioID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipes", x => x.EquipeID);
                    table.ForeignKey(
                        name: "FK_Equipes_Unidades_UnidadeID",
                        column: x => x.UnidadeID,
                        principalTable: "Unidades",
                        principalColumn: "UnidadeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: false),
                    CPF = table.Column<string>(type: "TEXT", nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", nullable: false),
                    EquipeID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Equipes_EquipeID",
                        column: x => x.EquipeID,
                        principalTable: "Equipes",
                        principalColumn: "EquipeID");
                });

            migrationBuilder.CreateTable(
                name: "UnidadeUsuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "INTEGER", nullable: false),
                    UnidadeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadeUsuarios", x => new { x.UsuarioID, x.UnidadeID });
                    table.ForeignKey(
                        name: "FK_UnidadeUsuarios_Unidades_UnidadeID",
                        column: x => x.UnidadeID,
                        principalTable: "Unidades",
                        principalColumn: "UnidadeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnidadeUsuarios_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEpis",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "INTEGER", nullable: false),
                    EpiID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEpis", x => new { x.UsuarioID, x.EpiID });
                    table.ForeignKey(
                        name: "FK_UsuarioEpis_Epis_EpiID",
                        column: x => x.EpiID,
                        principalTable: "Epis",
                        principalColumn: "EpiID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioEpis_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_LiderUsuarioID",
                table: "Equipes",
                column: "LiderUsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_UnidadeID",
                table: "Equipes",
                column: "UnidadeID");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadeUsuarios_UnidadeID",
                table: "UnidadeUsuarios",
                column: "UnidadeID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEpis_EpiID",
                table: "UsuarioEpis",
                column: "EpiID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EquipeID",
                table: "Usuarios",
                column: "EquipeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipes_Usuarios_LiderUsuarioID",
                table: "Equipes",
                column: "LiderUsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Unidades_UnidadeID",
                table: "Equipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Usuarios_LiderUsuarioID",
                table: "Equipes");

            migrationBuilder.DropTable(
                name: "UnidadeUsuarios");

            migrationBuilder.DropTable(
                name: "UsuarioEpis");

            migrationBuilder.DropTable(
                name: "Epis");

            migrationBuilder.DropTable(
                name: "Unidades");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Equipes");
        }
    }
}
