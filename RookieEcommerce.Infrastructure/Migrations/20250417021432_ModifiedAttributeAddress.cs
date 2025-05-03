using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAttributeAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress_Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_CreatedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_HouseNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_ModifiedDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Street",
                table: "Orders",
                newName: "ShippingAddress_StreetAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_StreetAddress",
                table: "Orders",
                newName: "ShippingAddress_Street");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Country",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingAddress_CreatedDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_HouseNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShippingAddress_Id",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingAddress_ModifiedDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}