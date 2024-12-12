using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTech.Migrations
{
    /// <inheritdoc />
    public partial class ClassScheduleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedule_Classes_ClassId",
                table: "ClassSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassSchedule",
                table: "ClassSchedule");

            migrationBuilder.RenameTable(
                name: "ClassSchedule",
                newName: "ClassSchedules");

            migrationBuilder.RenameIndex(
                name: "IX_ClassSchedule_ClassId",
                table: "ClassSchedules",
                newName: "IX_ClassSchedules_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassSchedules",
                table: "ClassSchedules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Classes_ClassId",
                table: "ClassSchedules",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Classes_ClassId",
                table: "ClassSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassSchedules",
                table: "ClassSchedules");

            migrationBuilder.RenameTable(
                name: "ClassSchedules",
                newName: "ClassSchedule");

            migrationBuilder.RenameIndex(
                name: "IX_ClassSchedules_ClassId",
                table: "ClassSchedule",
                newName: "IX_ClassSchedule_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassSchedule",
                table: "ClassSchedule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedule_Classes_ClassId",
                table: "ClassSchedule",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
