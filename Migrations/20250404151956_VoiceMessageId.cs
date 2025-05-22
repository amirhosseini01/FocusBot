using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusBot.Migrations
{
    /// <inheritdoc />
    public partial class VoiceMessageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoiceMessageId",
                table: "Lotteries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceMessageId",
                table: "Lotteries");
        }
    }
}
