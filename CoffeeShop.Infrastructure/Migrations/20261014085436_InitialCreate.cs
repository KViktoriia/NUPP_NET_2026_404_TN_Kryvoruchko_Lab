using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Baristas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ExperienceYears = table.Column<int>(type: "INTEGER", nullable: false),
                    Shift = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsCertified = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baristas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beverages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    VolumeMl = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beverages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReceiptNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BaristaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderReceipts_Baristas_BaristaId",
                        column: x => x.BaristaId,
                        principalTable: "Baristas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeBeverages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoffeeBeansOrigin = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MilkType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CaffeineAmountMg = table.Column<int>(type: "INTEGER", nullable: false),
                    HasSyrup = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeBeverages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeBeverages_Beverages_Id",
                        column: x => x.Id,
                        principalTable: "Beverages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeaBeverages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeaVariety = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BrewingTemperatureC = table.Column<int>(type: "INTEGER", nullable: false),
                    SteepingTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    HasHoney = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeaBeverages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeaBeverages_Beverages_Id",
                        column: x => x.Id,
                        principalTable: "Beverages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderBeverages",
                columns: table => new
                {
                    OrderReceiptId = table.Column<int>(type: "INTEGER", nullable: false),
                    BeverageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBeverages", x => new { x.OrderReceiptId, x.BeverageId });
                    table.ForeignKey(
                        name: "FK_OrderBeverages_Beverages_BeverageId",
                        column: x => x.BeverageId,
                        principalTable: "Beverages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderBeverages_OrderReceipts_OrderReceiptId",
                        column: x => x.OrderReceiptId,
                        principalTable: "OrderReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderBeverages_BeverageId",
                table: "OrderBeverages",
                column: "BeverageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReceipts_BaristaId",
                table: "OrderReceipts",
                column: "BaristaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoffeeBeverages");

            migrationBuilder.DropTable(
                name: "OrderBeverages");

            migrationBuilder.DropTable(
                name: "TeaBeverages");

            migrationBuilder.DropTable(
                name: "OrderReceipts");

            migrationBuilder.DropTable(
                name: "Beverages");

            migrationBuilder.DropTable(
                name: "Baristas");
        }
    }
}
