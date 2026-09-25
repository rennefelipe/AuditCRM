using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditCRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingOperationalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiName",
                table: "Shoppings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ControlShopUrl",
                table: "Shoppings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortalUrl",
                table: "Shoppings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationEmail",
                table: "Shoppings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Shoppings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XmlReadingEmail",
                table: "Shoppings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiName",
                table: "Shoppings");

            migrationBuilder.DropColumn(
                name: "ControlShopUrl",
                table: "Shoppings");

            migrationBuilder.DropColumn(
                name: "PortalUrl",
                table: "Shoppings");

            migrationBuilder.DropColumn(
                name: "RegistrationEmail",
                table: "Shoppings");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Shoppings");

            migrationBuilder.DropColumn(
                name: "XmlReadingEmail",
                table: "Shoppings");
        }
    }
}
