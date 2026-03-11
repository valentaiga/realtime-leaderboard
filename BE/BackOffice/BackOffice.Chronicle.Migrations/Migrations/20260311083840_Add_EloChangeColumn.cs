using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOffice.Chronicle.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Add_EloChangeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "elo_change",
                table: "match_players",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "elo_change",
                table: "match_players");
        }
    }
}
