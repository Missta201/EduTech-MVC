using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTech.Migrations
{
    /// <inheritdoc />
    public partial class ModifyInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Invoices");
        }
    }
}
