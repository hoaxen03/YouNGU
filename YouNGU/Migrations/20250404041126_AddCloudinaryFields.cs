using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNGU.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudinaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Videos",
                newName: "VideoPath");

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryUrl",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPublicId",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Likes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "CloudinaryUrl",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ThumbnailPublicId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "VideoPath",
                table: "Videos",
                newName: "FilePath");
        }
    }
}
