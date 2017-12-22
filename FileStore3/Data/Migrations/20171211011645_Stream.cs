using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FileStore3.Data.Migrations
{
    public partial class Stream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "File");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "File",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "File");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "File",
                nullable: true);
        }
    }
}
