using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatingFromFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "feedback");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "feedback",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
