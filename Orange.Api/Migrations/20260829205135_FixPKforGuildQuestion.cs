using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Orange.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPKforGuildQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildQuestions_Guilds_GuildId1",
                table: "GuildQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildQuestions",
                table: "GuildQuestions");

            migrationBuilder.DropIndex(
                name: "IX_GuildQuestions_GuildId1",
                table: "GuildQuestions");

            migrationBuilder.DropColumn(
                name: "GuildId1",
                table: "GuildQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GuildQuestions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildQuestions",
                table: "GuildQuestions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildQuestions_Guilds_GuildId",
                table: "GuildQuestions",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildQuestions_Guilds_GuildId",
                table: "GuildQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildQuestions",
                table: "GuildQuestions");

            migrationBuilder.DropIndex(
                name: "IX_GuildQuestions_GuildId",
                table: "GuildQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GuildQuestions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildId1",
                table: "GuildQuestions",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildQuestions",
                table: "GuildQuestions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQuestions_GuildId1",
                table: "GuildQuestions",
                column: "GuildId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildQuestions_Guilds_GuildId1",
                table: "GuildQuestions",
                column: "GuildId1",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
