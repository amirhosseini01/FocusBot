using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusBot.Migrations
{
    /// <inheritdoc />
    public partial class ChannelMessageIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArchiveMessageId",
                table: "Lotteries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelMessageId",
                table: "Lotteries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveMessageId",
                table: "Lotteries");

            migrationBuilder.DropColumn(
                name: "ChannelMessageId",
                table: "Lotteries");
        }
    }
}
