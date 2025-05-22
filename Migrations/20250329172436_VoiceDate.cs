using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusBot.Migrations
{
    /// <inheritdoc />
    public partial class VoiceDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SendVoiceDate",
                table: "Lotteries",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendVoiceDate",
                table: "Lotteries");
        }
    }
}
