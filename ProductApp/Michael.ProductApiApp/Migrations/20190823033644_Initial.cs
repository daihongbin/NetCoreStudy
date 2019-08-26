using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Michael.ProductApiApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false),
                    ParentID = table.Column<int>(nullable: false),
                    CategoryName = table.Column<string>(nullable: true),
                    ViewOrder = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    GoodsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodsSN = table.Column<string>(nullable: true),
                    GoodsName = table.Column<string>(nullable: true),
                    CategoryID = table.Column<int>(nullable: false),
                    MarketPrice = table.Column<decimal>(nullable: false),
                    SalePrice = table.Column<decimal>(nullable: false),
                    Img = table.Column<string>(nullable: true),
                    OnSale = table.Column<bool>(nullable: false),
                    InputTime = table.Column<DateTime>(nullable: false),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    IsReal = table.Column<bool>(nullable: false),
                    BarCode = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.GoodsID);
                    table.ForeignKey(
                        name: "FK_Goods_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goods_CategoryID",
                table: "Goods",
                column: "CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
