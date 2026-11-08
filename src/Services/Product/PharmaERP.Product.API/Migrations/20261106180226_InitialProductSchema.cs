using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaERP.Product.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GenericName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DosageForm = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Strength = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HSNCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GstPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IsPrescriptionRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryCode",
                table: "ProductCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryName",
                table: "ProductCategories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId_IsActive",
                table: "Products",
                columns: new[] { "ProductCategoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
