using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace genzcoders.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Account",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GoogleEmail",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordChangedAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Account",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiresAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSecret",
                table: "Account",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailOtp",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOtp", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_GoogleId",
                table: "Account",
                column: "GoogleId",
                unique: true,
                filter: "[GoogleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Account_PasswordResetToken",
                table: "Account",
                column: "PasswordResetToken",
                filter: "[PasswordResetToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtp_Email_OtpCode",
                table: "EmailOtp",
                columns: new[] { "Email", "OtpCode" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtp_ExpiresAt",
                table: "EmailOtp",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOtp");

            migrationBuilder.DropIndex(
                name: "IX_Account_GoogleId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_PasswordResetToken",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "GoogleEmail",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PasswordChangedAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiresAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "TwoFactorSecret",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
