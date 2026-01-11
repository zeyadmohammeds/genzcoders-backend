using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace genzcoders.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Week",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseRoundId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BusinessEntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Week", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Week_CourseRound",
                        column: x => x.CourseRoundId,
                        principalTable: "CourseRound",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Week_CourseRoundId_BusinessEntityName",
                table: "Week",
                columns: new[] { "CourseRoundId", "BusinessEntityName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Week");
        }
    }
}
