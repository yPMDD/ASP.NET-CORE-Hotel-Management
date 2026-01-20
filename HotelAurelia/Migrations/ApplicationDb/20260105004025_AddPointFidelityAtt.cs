using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAurelia.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddPointFidelityAtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pointsFidelite",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pointsFidelite",
                table: "AspNetUsers");
        }
    }
}
