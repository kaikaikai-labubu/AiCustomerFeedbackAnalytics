using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalColumnsForFeedbackAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Confidence",
                table: "analysis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryTopic",
                table: "analysis",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "analysis",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confidence",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "PrimaryTopic",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "analysis");
        }
    }
}
