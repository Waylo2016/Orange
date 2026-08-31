using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orange.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionOrder",
                table: "GuildQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionOrder",
                table: "GuildQuestions");
        }
    }
}
