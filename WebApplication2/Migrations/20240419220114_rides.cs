using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class rides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Driver_DriverEmail",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Passanger_PassangerEmail",
                table: "Rides");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Driver_DriverEmail",
                table: "Rides",
                column: "DriverEmail",
                principalTable: "Driver",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Passanger_PassangerEmail",
                table: "Rides",
                column: "PassangerEmail",
                principalTable: "Passanger",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Driver_DriverEmail",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Passanger_PassangerEmail",
                table: "Rides");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Driver_DriverEmail",
                table: "Rides",
                column: "DriverEmail",
                principalTable: "Driver",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Passanger_PassangerEmail",
                table: "Rides",
                column: "PassangerEmail",
                principalTable: "Passanger",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
