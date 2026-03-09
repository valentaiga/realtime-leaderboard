using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOffice.Chronicle.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Update_UniqueMatchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_matches_match_id",
                table: "matches");

            migrationBuilder.AlterColumn<string>(
                name: "match_id",
                table: "matches",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_matches_match_id",
                table: "matches",
                column: "match_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_matches_match_id",
                table: "matches");

            migrationBuilder.AlterColumn<Guid>(
                name: "match_id",
                table: "matches",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.CreateIndex(
                name: "IX_matches_match_id",
                table: "matches",
                column: "match_id");
        }
    }
}
