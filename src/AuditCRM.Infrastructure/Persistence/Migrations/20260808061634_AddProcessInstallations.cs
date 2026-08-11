using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessInstallations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessInstallations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsibleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ErpId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Successful = table.Column<bool>(type: "bit", nullable: false),
                    XmlLocationType = table.Column<int>(type: "int", nullable: false),
                    NumberOfRegisters = table.Column<int>(type: "int", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessInstallations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessInstallations_Erps_ErpId",
                        column: x => x.ErpId,
                        principalTable: "Erps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProcessInstallations_StoreProcesses_StoreProcessId",
                        column: x => x.StoreProcessId,
                        principalTable: "StoreProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessInstallations_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_ErpId",
                table: "ProcessInstallations",
                column: "ErpId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_InstalledAt",
                table: "ProcessInstallations",
                column: "InstalledAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_ResponsibleUserId",
                table: "ProcessInstallations",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_StoreProcessId",
                table: "ProcessInstallations",
                column: "StoreProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_Successful",
                table: "ProcessInstallations",
                column: "Successful");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstallations_XmlLocationType",
                table: "ProcessInstallations",
                column: "XmlLocationType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessInstallations");
        }
    }
}
