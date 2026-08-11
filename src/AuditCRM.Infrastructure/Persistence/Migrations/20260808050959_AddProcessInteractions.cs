using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsibleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NextAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextActionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_ProcessInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessInteractions_StoreContacts_StoreContactId",
                        column: x => x.StoreContactId,
                        principalTable: "StoreContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProcessInteractions_StoreProcesses_StoreProcessId",
                        column: x => x.StoreProcessId,
                        principalTable: "StoreProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessInteractions_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInteractions_Channel",
                table: "ProcessInteractions",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInteractions_OccurredAt",
                table: "ProcessInteractions",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInteractions_ResponsibleUserId",
                table: "ProcessInteractions",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInteractions_StoreContactId",
                table: "ProcessInteractions",
                column: "StoreContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInteractions_StoreProcessId",
                table: "ProcessInteractions",
                column: "StoreProcessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessInteractions");
        }
    }
}
