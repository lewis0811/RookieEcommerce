using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductVariantType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VariantType",
                table: "ProductVariants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VariantType",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}