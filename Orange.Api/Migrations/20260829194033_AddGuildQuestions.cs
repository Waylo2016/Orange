using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orange.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Guilds_GuildId",
                table: "Guilds",
                column: "GuildId");

            migrationBuilder.CreateTable(
                name: "GuildQuestions",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    GuildId1 = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildQuestions", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_GuildQuestions_Guilds_GuildId1",
                        column: x => x.GuildId1,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId1",
                table: "GuildQuestions",
                column: "GuildId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildQuestions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Guilds_GuildId",
                table: "Guilds");
        }
    }
}
