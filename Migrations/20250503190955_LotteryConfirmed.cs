using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusBot.Migrations
{
    /// <inheritdoc />
    public partial class LotteryConfirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Lotteries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Lotteries");
        }
    }
}
