using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingForFeedbackAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float[]>(
                name: "Embedding",
                table: "analysis",
                type: "real[]",
                nullable: false,
                defaultValue: new float[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "analysis");
        }
    }
}
