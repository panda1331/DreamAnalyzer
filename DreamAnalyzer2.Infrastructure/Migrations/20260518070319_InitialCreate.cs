using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAnalyzer2.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DreamSymbols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DreamSymbols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dreams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DreamDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dreams_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DreamAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Interpretation = table.Column<string>(type: "text", nullable: false),
                    MoodName = table.Column<string>(type: "text", nullable: false),
                    MoodColorHex = table.Column<string>(type: "text", nullable: false),
                    MoodDescription = table.Column<string>(type: "text", nullable: false),
                    DreamId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DreamAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DreamAnalyses_Dreams_DreamId",
                        column: x => x.DreamId,
                        principalTable: "Dreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DreamAnalyses_Dreams_DreamId1",
                        column: x => x.DreamId1,
                        principalTable: "Dreams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnalysisSymbols",
                columns: table => new
                {
                    DreamAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    SymbolsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisSymbols", x => new { x.DreamAnalysisId, x.SymbolsId });
                    table.ForeignKey(
                        name: "FK_AnalysisSymbols_DreamAnalyses_DreamAnalysisId",
                        column: x => x.DreamAnalysisId,
                        principalTable: "DreamAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisSymbols_DreamSymbols_SymbolsId",
                        column: x => x.SymbolsId,
                        principalTable: "DreamSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisSymbols_SymbolsId",
                table: "AnalysisSymbols",
                column: "SymbolsId");

            migrationBuilder.CreateIndex(
                name: "IX_DreamAnalyses_DreamId",
                table: "DreamAnalyses",
                column: "DreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DreamAnalyses_DreamId1",
                table: "DreamAnalyses",
                column: "DreamId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dreams_UserId",
                table: "Dreams",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisSymbols");

            migrationBuilder.DropTable(
                name: "DreamAnalyses");

            migrationBuilder.DropTable(
                name: "DreamSymbols");

            migrationBuilder.DropTable(
                name: "Dreams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
