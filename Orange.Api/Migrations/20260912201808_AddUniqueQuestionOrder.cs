using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orange.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueQuestionOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId_QuestionOrder",
                table: "GuildQuestions",
                columns: new[] { "GuildId", "QuestionOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildQuestions_GuildId_QuestionOrder",
                table: "GuildQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions",
                column: "GuildId");
        }
    }
}
