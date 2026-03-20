using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Larmcentralen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteSolutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Alarms_AlarmId",
                table: "Solutions");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Alarms_AlarmId",
                table: "Solutions",
                column: "AlarmId",
                principalTable: "Alarms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Alarms_AlarmId",
                table: "Solutions");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Alarms_AlarmId",
                table: "Solutions",
                column: "AlarmId",
                principalTable: "Alarms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
