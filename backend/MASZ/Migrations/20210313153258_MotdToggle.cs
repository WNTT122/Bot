﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MASZ.Migrations
{
    public partial class MotdToggle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowMotd",
                table: "GuildMotds",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowMotd",
                table: "GuildMotds");
        }
    }
}
