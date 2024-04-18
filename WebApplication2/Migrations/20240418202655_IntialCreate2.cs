using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passanger_Credentials_Email",
                table: "Passanger");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Passanger_Credentials_Email",
                table: "Passanger",
                column: "Email",
                principalTable: "Credentials",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
