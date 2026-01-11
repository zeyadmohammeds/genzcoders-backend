using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace genzcoders.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialsZoomMeetingsAndCourseRoundStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseRoundStudent",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseRoundId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRoundStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseRoundStudent_CourseRound",
                        column: x => x.CourseRoundId,
                        principalTable: "CourseRound",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseRoundStudent_Student",
                        column: x => x.StudentId,
                        principalTable: "Account",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseRoundId = table.Column<long>(type: "bigint", nullable: false),
                    InstructorId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MaterialType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_CourseRound",
                        column: x => x.CourseRoundId,
                        principalTable: "CourseRound",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Material_Instructor",
                        column: x => x.InstructorId,
                        principalTable: "Account",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ZoomMeeting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseRoundId = table.Column<long>(type: "bigint", nullable: false),
                    InstructorId = table.Column<long>(type: "bigint", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MeetingId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Passcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeetingDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoomMeeting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoomMeeting_CourseRound",
                        column: x => x.CourseRoundId,
                        principalTable: "CourseRound",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ZoomMeeting_Instructor",
                        column: x => x.InstructorId,
                        principalTable: "Account",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseRoundStudent_CourseRoundId_StudentId",
                table: "CourseRoundStudent",
                columns: new[] { "CourseRoundId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseRoundStudent_StudentId",
                table: "CourseRoundStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_CourseRoundId",
                table: "Material",
                column: "CourseRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_InstructorId",
                table: "Material",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoomMeeting_CourseRoundId",
                table: "ZoomMeeting",
                column: "CourseRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoomMeeting_InstructorId",
                table: "ZoomMeeting",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseRoundStudent");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "ZoomMeeting");
        }
    }
}
