using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class addRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Driver_Credentials_Email",
                table: "Driver");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Driver_Credentials_Email",
                table: "Driver",
                column: "Email",
                principalTable: "Credentials",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
