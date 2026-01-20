using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAurelia.Migrations
{
    /// <inheritdoc />
    public partial class AddingReceptionnisteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Receptionniste",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "User",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Poste",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d2e4b8a0-5c3d-4e2f-9a8b-1f2e3d4c5b6a", "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Receptionniste", "RECEPTIONNISTE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Poste",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Receptionniste");
        }
    }
}
