using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOffice.Chronicle.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Create_MatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE SEQUENCE MatchSeq START WITH 1 INCREMENT BY 1");

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                });

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
            migrationBuilder.Sql("DROP INDEX IF EXISTS MatchSeq");
            migrationBuilder.DropTable(
                name: "matches");
        }
    }
}
