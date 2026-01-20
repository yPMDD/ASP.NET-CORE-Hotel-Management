using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAurelia.Migrations
{
    /// <inheritdoc />
    public partial class addingimageattribuetochambremodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "Chambres",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image",
                table: "Chambres");
        }
    }
}
