using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduTech.Migrations
{
    /// <inheritdoc />
    public partial class modifyClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "24b7594d-25d3-4b2d-b634-cfffc40ce42f");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "335cad7c-e3fa-4a85-962a-cf65d65ee7ab");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "e0dd9f76-d78f-4b2d-a889-94c1a08c1bac");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfStudents",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClassLecturers",
                columns: table => new
                {
                    ClassesTeachingId = table.Column<int>(type: "int", nullable: false),
                    LecturersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLecturers", x => new { x.ClassesTeachingId, x.LecturersId });
                    table.ForeignKey(
                        name: "FK_ClassLecturers_Classes_ClassesTeachingId",
                        column: x => x.ClassesTeachingId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassLecturers_Users_LecturersId",
                        column: x => x.LecturersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassStudents",
                columns: table => new
                {
                    ClassesAttendingId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStudents", x => new { x.ClassesAttendingId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_ClassStudents_Classes_ClassesAttendingId",
                        column: x => x.ClassesAttendingId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassStudents_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLecturers_LecturersId",
                table: "ClassLecturers",
                column: "LecturersId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_StudentsId",
                table: "ClassStudents",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassLecturers");

            migrationBuilder.DropTable(
                name: "ClassStudents");

            migrationBuilder.DropColumn(
                name: "NumberOfStudents",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Classes");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "24b7594d-25d3-4b2d-b634-cfffc40ce42f", 0, "bb6e9dab-2d8e-422e-9cbd-2c7600a55a74", "giaovu@edutech.com", true, false, null, "Giáo vụ 1", "GIAOVU@EDUTECH.COM", "GIAOVU@EDUTECH.COM", "AQAAAAIAAYagAAAAELLAa62GDbmAe5h+ZCMN0tB+DspKfV+Svl5Mr6NI1eaH2n9pVcAkWgYUyxrYhxEZ4g==", null, false, "48821a54-ec74-475f-860c-db5f145ad2c4", false, "giaovu@edutech.com", "Scheduler" },
                    { "335cad7c-e3fa-4a85-962a-cf65d65ee7ab", 0, "f91a16ec-aa5a-4e87-880b-72defbd244d5", "admin@edutech.com", true, false, null, "Admin User", "ADMIN@EDUTECH.COM", "ADMIN@EDUTECH.COM", "AQAAAAIAAYagAAAAEHx1Wq/9OxDvXzAqbBlXvWT0zrw2uTPOt4FDYtB4rW4qlIrddD8xvDWsj3ZbuBb9SA==", null, false, "fe84cc24-1b8d-4851-9981-2aa38af210c6", false, "admin@edutech.com", "Admin" },
                    { "e0dd9f76-d78f-4b2d-a889-94c1a08c1bac", 0, "be14f35f-3ffb-4358-8585-0644901ebf5d", "giangvien@edutech.com", true, false, null, "Giảng viên 1", "GIANGVIEN@EDUTECH.COM", "GIANGVIEN@EDUTECH.COM", "AQAAAAIAAYagAAAAEN+etor16HvQjnSYXDG2AzGNs0p3WoCg0iQccnOg1vwWg8OBiON5HhEvVOcEfnntsQ==", null, false, "eed0f7ab-d665-44ff-ad78-ccd24cdd28e3", false, "giangvien@edutech.com", "Lecturer" }
                });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "UserType", "Admin", "335cad7c-e3fa-4a85-962a-cf65d65ee7ab" },
                    { 2, "UserType", "Scheduler", "24b7594d-25d3-4b2d-b634-cfffc40ce42f" },
                    { 3, "UserType", "Lecturer", "e0dd9f76-d78f-4b2d-a889-94c1a08c1bac" }
                });
        }
    }
}
