using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNGU.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamingProfileToVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreamingProfile",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamingProfile",
                table: "Videos");
        }
    }
}
