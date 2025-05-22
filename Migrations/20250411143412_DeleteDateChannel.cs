using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusBot.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDateChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteFromArchiveChannelDate",
                table: "Lotteries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteFromMainChannelDate",
                table: "Lotteries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendToArchiveChannelDate",
                table: "Lotteries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendToMainChannelDate",
                table: "Lotteries",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteFromArchiveChannelDate",
                table: "Lotteries");

            migrationBuilder.DropColumn(
                name: "DeleteFromMainChannelDate",
                table: "Lotteries");

            migrationBuilder.DropColumn(
                name: "SendToArchiveChannelDate",
                table: "Lotteries");

            migrationBuilder.DropColumn(
                name: "SendToMainChannelDate",
                table: "Lotteries");
        }
    }
}
