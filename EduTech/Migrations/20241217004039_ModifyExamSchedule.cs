using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTech.Migrations
{
    /// <inheritdoc />
    public partial class ModifyExamSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "ExamSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "ExamSchedules");
        }
    }
}
