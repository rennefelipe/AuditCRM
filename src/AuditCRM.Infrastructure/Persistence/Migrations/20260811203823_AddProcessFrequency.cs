using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessFrequency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "StoreProcesses",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProcesses_Frequency",
                table: "StoreProcesses",
                column: "Frequency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreProcesses_Frequency",
                table: "StoreProcesses");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "StoreProcesses");
        }
    }
}
