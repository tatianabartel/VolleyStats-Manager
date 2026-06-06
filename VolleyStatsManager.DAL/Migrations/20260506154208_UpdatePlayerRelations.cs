using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolleyStatsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlayerRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_UserId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId1",
                table: "PlayerStats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserId",
                table: "Teams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_PlayerId1",
                table: "PlayerStats",
                column: "PlayerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStats_Players_PlayerId1",
                table: "PlayerStats",
                column: "PlayerId1",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStats_Players_PlayerId1",
                table: "PlayerStats");

            migrationBuilder.DropIndex(
                name: "IX_Teams_UserId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStats_PlayerId1",
                table: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "PlayerId1",
                table: "PlayerStats");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserId",
                table: "Teams",
                column: "UserId",
                unique: true);
        }
    }
}
