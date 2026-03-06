using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOffice.Chronicle.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Create_MatchTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE SEQUENCE MatchSeq START WITH 1 INCREMENT BY 1");

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "match_players",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    is_win = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_players", x => new { x.player_id, x.match_id });
                    table.ForeignKey(
                        name: "FK_match_players_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_players_match_id",
                table: "match_players",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_players_player_id",
                table: "match_players",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_players_player_id_is_win",
                table: "match_players",
                columns: new[] { "player_id", "is_win" });

            migrationBuilder.CreateIndex(
                name: "IX_matches_finished_at",
                table: "matches",
                column: "finished_at");

            migrationBuilder.CreateIndex(
                name: "IX_matches_match_id",
                table: "matches",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_started_at",
                table: "matches",
                column: "started_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS MatchSeq");

            migrationBuilder.DropTable(
                name: "match_players");

            migrationBuilder.DropTable(
                name: "matches");
        }
    }
}
