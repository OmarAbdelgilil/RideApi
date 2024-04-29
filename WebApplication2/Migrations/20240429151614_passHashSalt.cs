using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class passHashSalt : Migration
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

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Credentials");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Credentials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Credentials",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Credentials",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Driver_DriverEmail",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Passanger_PassangerEmail",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Credentials");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Credentials",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Credentials",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
