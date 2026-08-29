using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orange.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixKeyForQuesions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GuildQuestions_GuildId",
                table: "GuildQuestions",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GuildQuestions_GuildId",
                table: "GuildQuestions");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions",
                column: "GuildId");
        }
    }
}
