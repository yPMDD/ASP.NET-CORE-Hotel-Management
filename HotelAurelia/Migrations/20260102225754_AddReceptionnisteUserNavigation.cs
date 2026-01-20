using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAurelia.Migrations
{
    /// <inheritdoc />
    public partial class AddReceptionnisteUserNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Receptionnistes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Receptionnistes_UserId1",
                table: "Receptionnistes",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Receptionnistes_User_UserId1",
                table: "Receptionnistes",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receptionnistes_User_UserId1",
                table: "Receptionnistes");

            migrationBuilder.DropIndex(
                name: "IX_Receptionnistes_UserId1",
                table: "Receptionnistes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Receptionnistes");
        }
    }
}
