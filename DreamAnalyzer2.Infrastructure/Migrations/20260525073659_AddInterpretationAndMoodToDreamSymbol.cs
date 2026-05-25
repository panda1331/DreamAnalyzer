using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAnalyzer2.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterpretationAndMoodToDreamSymbol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Interpretation",
                table: "DreamSymbols",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "DreamSymbols",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interpretation",
                table: "DreamSymbols");

            migrationBuilder.DropColumn(
                name: "Mood",
                table: "DreamSymbols");
        }
    }
}
