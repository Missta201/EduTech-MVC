using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduTech.Migrations
{
    /// <inheritdoc />
    public partial class UserTypesSeedDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "1", 0, "7c15be29-80c8-4958-91d8-513755bfb6dc", "admin@edutech.com", true, false, null, "Admin User", "ADMIN@EDUTECH.COM", "ADMIN@EDUTECH.COM", "AQAAAAIAAYagAAAAEKWxWlXCsDK3zkUQ2T7Zu3M3g34axcBcGe332J/Pf7stFXoqxB4sWByAIiyuASlbnQ==", null, false, "abe58797-db48-4140-b2a4-410db5ef4e92", false, "admin@edutech.com", "Admin" },
                    { "2", 0, "00b7a3fe-639d-4781-977e-818e866293d9", "giaovu@edutech.com", true, false, null, "Giáo vụ 1", "GIAOVU@EDUTECH.COM", "GIAOVU@EDUTECH.COM", "AQAAAAIAAYagAAAAEMbghaqAR87/K9DmtOqI3npJ4vzbATSy2gB/PPS3hEnoC96MohzS/XvMSTI7hhyoUQ==", null, false, "11bbdd48-a3a0-468f-b2f7-196ce23df791", false, "giaovu@edutech.com", "Scheduler" },
                    { "3", 0, "e87ea7d8-8441-4aea-9fd6-ef1bc2c286c5", "giangvien@edutech.com", true, false, null, "Giảng viên 1", "GIANGVIEN@EDUTECH.COM", "GIANGVIEN@EDUTECH.COM", "AQAAAAIAAYagAAAAEN/7FSGhysLbASYaathc2Rs0/g2V0IddPW9T5KGxZamKGRvGh3eIGpDN30lJ9ciiTw==", null, false, "8db36db5-f9d7-43b5-ab82-1bb16a013ea4", false, "giangvien@edutech.com", "Lecturer" }
                });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "UserType", "Admin", "1" },
                    { 2, "UserType", "Scheduler", "2" },
                    { 3, "UserType", "Lecturer", "3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");
        }
    }
}
