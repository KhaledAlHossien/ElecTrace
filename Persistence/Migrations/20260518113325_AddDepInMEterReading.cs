using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepInMEterReading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_DepartmentId",
                table: "MeterReadings",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Departments_DepartmentId",
                table: "MeterReadings",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Departments_DepartmentId",
                table: "MeterReadings");

            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_DepartmentId",
                table: "MeterReadings");
        }
    }
}
